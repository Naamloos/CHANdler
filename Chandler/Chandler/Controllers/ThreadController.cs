using Chandler.Data;
using Chandler.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

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
                    var res = await http.PostAsync($"https://discordapp.com/api/webhooks/{sub.WebhookId}/{sub.Token}", new StringContent(jsondata, Encoding.UTF8, "application/json"));
                    var cont = await res.Content.ReadAsStringAsync();
                }
            }
        }

        /// <summary>
        /// Returns a list of threads for a given board
        /// </summary>
        /// <param name="tag">The board tag</param>
        /// <param name="includechildren">Whether or not to include the children to a thread</param>
        /// <returns>IEnunmerable of Thread or empty</returns>
        /// <response code="200">If the board was found successfully</response>
        [HttpGet, ProducesResponseType(200)]
        public ActionResult<IEnumerable<Thread>> GetThreads([FromQuery]string tag, [FromQuery]bool includechildren = false)
        {
            var boardthreads = ctx.Threads.Where(x => x.BoardTag == tag && x.ParentId == -1);
            if (boardthreads.Count() > 0)
            {
                if (!includechildren) return boardthreads.OrderBy(x => x.Id).ToList();
                else
                {
                    var childedthreads = boardthreads.OrderBy(x => x.Id).ToList();
                    childedthreads.ForEach(x => x.ChildThreads = boardthreads.Where(a => a.ParentId == x.Id));
                    return childedthreads;
                }
            }
            else return this.NotFound("Board not found or the board has no threads");
        }

        /// <summary>
        /// Gets a thread by its ID
        /// </summary>
        /// <param name="id">The ID of the thread</param>
        /// <param name="includechildren">Whether or not to include the children to a thread</param>
        /// <returns>Thread or empty</returns>
        /// <response code="200">If the thread was found</response>
        [HttpGet("single"), ProducesResponseType(200)]
        public ActionResult<Thread> GetSingleThread([FromQuery]int id, [FromQuery]bool includechildren = false)
        {
            var thread = ctx.Threads.FirstOrDefault(x => x.Id == id);
            if (thread != null)
            {
                if (!includechildren) return thread;
                else
                {
                    thread.ChildThreads = ctx.Threads.Where(x => x.ParentId == thread.Id);
                    return thread;
                }
            }
            else return NotFound("No thread with the given ID was found");
        }

        /// <summary>
        /// Gets child posts on a given thread
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of children threads (IEnumerable of Thread)</returns>
        /// <response code="200">If the thread was found</response>
        [HttpGet("children"), ProducesResponseType(200)]
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
            if (!string.IsNullOrWhiteSpace(passw))
            {
                var hash = Passworder.GenerateHash(passw, this.config.DefaultPassword);
                var newpass = new Password()
                {
                    Hash = hash.Hash,
                    Salt = hash.Salt
                };
                ctx.Passwords.Add(newpass);
                ctx.SaveChanges();
                passid = newpass.Id;
            }

            //var settings = new TextEncoderSettings();
            //settings.AllowRange(UnicodeRanges.All);
            //settings.ForbidCharacters(new[] { '<', '>' });
            //var encoder = HtmlEncoder.Create(settings);

            //newpost.Text = encoder.Encode(newpost.Text);
            //if (newpost.Image != null) newpost.Image = encoder.Encode(newpost.Image);
            //newpost.Username = encoder.Encode(newpost.Username);
            //if (newpost.Topic != null) newpost.Topic = encoder.Encode(newpost.Topic);

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
            var thread = ctx.Threads.FirstOrDefault(x => x.Id == postid);
            if (thread != null)
            {
                var passwd = ctx.Passwords.FirstOrDefault(x => x.Id == thread.PasswordId);
                if (passwd != null)
                {
                    var passcorrect = Passworder.VerifyPassword(pass, passwd.Hash, passwd.Salt, this.config.DefaultPassword);
                    if (passcorrect)
                    {
                        ctx.Threads.Remove(thread);
                        ctx.SaveChanges();
                        return Ok();
                    }
                }

                // failed, trying with master password
                var mpasswd = ctx.Passwords.First(x => x.Id == -1);
                var mpasscorrect = Passworder.VerifyPassword(pass, mpasswd.Hash, mpasswd.Salt, this.config.DefaultPassword);

                if (mpasscorrect)
                {
                    ctx.Threads.Remove(thread);
                    ctx.SaveChanges();
                    return Ok();
                }

                return BadRequest($"Received wrong password: {pass}");
            }
            else return NotFound($"Thread with the ID '{postid}' was not found");
        }
    }
}