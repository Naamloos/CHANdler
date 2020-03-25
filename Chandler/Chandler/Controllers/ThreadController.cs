using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using Chandler.Data;
using Chandler.Data.Entities;
using Chandler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyModel.Resolution;
using Newtonsoft.Json;

namespace Chandler.Controllers
{
    /// <summary>
    /// Api Controller For Threads
    /// </summary>
    [ApiController, Route("api/[controller]"), Produces("application/json")]
    public class ThreadController : Controller
    {
        private delegate Task PostCreatedEvent(Thread thread, DiscordWebhookBody body);
        private event PostCreatedEvent PostCreated;

        private readonly Database database;
        private readonly ServerMeta meta;
        private readonly ServerConfig config;
        private readonly DatabaseContext ctx;

        /// <summary>
        /// Thread API Ctor
        /// </summary>
        /// <param name="database"></param>
        /// <param name="meta"></param>
        /// <param name="conf"></param>
        public ThreadController(Database database, ServerMeta meta, ServerConfig conf)
        {
            this.database = database;
            this.meta = meta;
            this.config = conf;
            this.ctx = this.database.GetContext();
            this.ctx.Database.EnsureCreated();
            this.PostCreated += this.ThreadController_PostCreated;
        }

        private async Task ThreadController_PostCreated(Thread thread, DiscordWebhookBody body)
        {
            using var http = new HttpClient();
            foreach (var sub in ctx.WebhookSubscritptions)
            {
                if (thread.BoardTag == sub.BoardTag || thread.ParentId == sub.ThreadId)
                {
                    var jsondata = JsonConvert.SerializeObject(body);
                    var res = await http.PostAsync(sub.Url, new StringContent(jsondata, Encoding.UTF8, "application/json"));
                    var cont = await res.Content.ReadAsStringAsync();
                }
            }
        }

        /// <summary>
        /// Returns a list of threads for a given board
        /// </summary>
        /// <param name="tag">The board tag</param>
        /// <returns>IEnunmerable of Thread or empty</returns>
        /// <response code="200">If the board was found successfully</response>
        [HttpGet, ProducesResponseType(200)]
        public ActionResult<IEnumerable<Thread>> GetThreads([FromQuery]string tag)
        {
            if (ctx.Threads.Any(x => x.BoardTag == tag && x.ParentId == -1))
            {
                return ctx.Threads.Where(x => x.BoardTag == tag && x.ParentId == -1)
                    .OrderByDescending(x => ctx.Threads.Where(y => y.ParentId == x.Id || y.Id == x.Id).Select(y => y.Id).Max())
                    .ToList();
            }
            else return this.NotFound("not found");
        }

        /// <summary>
        /// Gets a thread by its ID
        /// </summary>
        /// <param name="id">The ID of the thread</param>
        /// <returns>Thread or empty</returns>
        /// <response code="200">If the thread was found</response>
        [HttpGet("single"), ProducesResponseType(200)]
        public ActionResult<Thread> GetSingleThread([FromQuery]int id) =>
            this.database.GetContext().Threads.FirstOrDefault(x => x.Id == id);

        /// <summary>
        /// Gets posts on a given thread
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of children threads (IEnumerable of Thread)</returns>
        /// <response code="200">If the thread was found</response>
        [HttpGet("posts"), ProducesResponseType(200)]
        public ActionResult<IEnumerable<Thread>> GetPosts([FromQuery]int id)
        {
            if (id < 1) return BadRequest("ID must be valid");
            else return this.ctx.Threads.Where(x => x.ParentId == id).OrderBy(x => x.Id).ToList();
        }

        /// <summary>
        /// Creates a post from post body content
        /// </summary>
        /// <param name="newpost">Thread Body</param>
        /// <returns>Newly posted thread</returns>
        /// <response code="200">If the thread was created successfully</response>
        [HttpPost("create"), ProducesResponseType(200)]
        public async Task<ActionResult<Thread>> CreatePost([FromBody] Thread newpost)
        {
            newpost.Id = 0;

            if (string.IsNullOrEmpty(newpost.Text))
                return BadRequest("Text too short");

            if (newpost.IsCommentReply && newpost.ParentId == -1)
                return BadRequest("Expected comment, got parent");

            if (string.IsNullOrEmpty(newpost.Username))
                newpost.Username = "Anonymous";

            var passw = newpost.GeneratePassword;
            int passid = -1;
            if (!string.IsNullOrEmpty(passw))
            {
                var salt = Passworder.GenerateSalt();
                var (hash, cycles) = Passworder.GenerateHash(passw, salt);
                var newpass = new Password()
                {
                    Salt = salt,
                    Hash = hash,
                    Cycles = cycles
                };
                ctx.Passwords.Add(newpass);
                ctx.SaveChanges();
                passid = newpass.Id;
            }

            var settings = new TextEncoderSettings();
            settings.AllowRange(UnicodeRanges.All);
            settings.ForbidCharacters(new[] { '<', '>' });
            var encoder = HtmlEncoder.Create(settings);

            newpost.Text = encoder.Encode(newpost.Text);
            if(newpost.Image != null) newpost.Image = encoder.Encode(newpost.Image);
            newpost.Username = encoder.Encode(newpost.Username);
            if (newpost.Topic != null) newpost.Topic = encoder.Encode(newpost.Topic);

            newpost.GeneratePassword = "";
            newpost.PasswordId = passid;

            await ctx.Threads.AddAsync(newpost);
            await ctx.SaveChangesAsync();

            /*            var body = new DiscordWebhookBody()
                        {
                            Content = $"**{newpost.Username}** Posted:",
                            Embed = new Embed()
                            {
                                Title = newpost.Topic,
                                Description = newpost.Text,
                                Url = new Uri($"{this.Request.Host}/posts?id={newpost.Id}")
                            }
                        };*/

            var body = new DiscordWebhookBody()
            {
                Content = $"**{newpost.Username}** Posted:\n__{newpost.Topic}__\n{newpost.Text}"
            };

            await this.PostCreated.Invoke(newpost, body);

            return newpost;
        }

        /// <summary>
        /// Delete a post via delete request type
        /// </summary>
        /// <param name="postid">ID of the most to delete</param>
        /// <param name="pass">Password to the thread</param>
        /// <returns>True if thread was deleted</returns>
        /// <response code="200">If the thread was created successfully</response>
        [HttpDelete("delete"), ProducesResponseType(200)]
        public ActionResult<bool> DeletePost([FromQuery]int postid = -1, [FromQuery]string pass = "")
        {
            if (ctx.Threads.Any(x => x.Id == postid))
            {
                var thread = ctx.Threads.First(x => x.Id == postid);
                var psasdsa = ctx.Passwords.ToArray();
                if (ctx.Passwords.Any(x => x.Id == thread.PasswordId))
                {
                    var passwd = ctx.Passwords.First(x => x.Id == thread.PasswordId);


                    bool passcorrect = Passworder.CompareHash(pass, passwd.Salt, passwd.Hash, passwd.Cycles);
                    if (passcorrect)
                    {
                        ctx.Threads.Remove(thread);
                        ctx.SaveChanges();
                        return Ok();
                    }
                }

                // failed, trying with master password
                var mpasswd = ctx.Passwords.First(x => x.Id == -1);
                bool mpasscorrect = Passworder.CompareHash(pass, mpasswd.Salt, mpasswd.Hash, mpasswd.Cycles);

                if (mpasscorrect)
                {
                    ctx.Threads.Remove(thread);
                    ctx.SaveChanges();
                    return Ok();
                }

                return BadRequest($"Received wrong password: {pass}");
            }
            else return NotFound();
        }
    }
}
