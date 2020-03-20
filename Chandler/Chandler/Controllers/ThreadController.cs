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
using Newtonsoft.Json;

namespace Chandler.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class ThreadController : Controller
    {
        private delegate Task PostCreatedEvent(Thread thread, DiscordWebhookBody body);
        private event PostCreatedEvent PostCreated;

        private readonly Database database;
        private readonly ServerMeta meta;
        private readonly ServerConfig config;
        private readonly DatabaseContext ctx;

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

        [HttpGet]
        public ActionResult<IEnumerable<Thread>> GetThreads(string tag = "")
        {
            if (ctx.Threads.Any(x => x.BoardTag == tag && x.ParentId == -1))
            {
                return ctx.Threads.Where(x => x.BoardTag == tag && x.ParentId == -1)
                    .OrderByDescending(x => ctx.Threads.Where(y => y.ParentId == x.Id || y.Id == x.Id).Select(y => y.Id).Max())
                    .ToList();
            }
            return this.NotFound("not found");
        }

        [HttpGet("single")]
        public ActionResult<Thread> GetSingleThread([FromQuery]int id) =>
            this.database.GetContext().Threads.FirstOrDefault(x => x.Id == id);

        [HttpGet("posts")]
        public ActionResult<IEnumerable<Thread>> GetPosts(int id = -1) => this.ctx.Threads.Where(x => x.ParentId == id).OrderBy(x => x.Id).ToList();

        [HttpPost("create")]
        public async Task<ActionResult<Thread>> CreatePost([FromBody] Thread newpost)
        {
            newpost.Id = 0;

            if (string.IsNullOrEmpty(newpost.Text))
                return BadRequest("Text too short");

            if (string.IsNullOrEmpty(newpost.Username))
                newpost.Username = "Anonymous";

            if (newpost.IsCommentReply && newpost.ParentId == -1)
                return BadRequest("Expected comment, got parent");

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

        [HttpGet("create")]
        public async Task<ActionResult<Thread>> CreatePostAndRedirect([FromQuery]string boardtag, [FromQuery]string text, [FromQuery]int parent_id = -1, [FromQuery]string username = null, [FromQuery]string topic = null, [FromQuery]string password = null, [FromQuery]string imageurl = null, [FromQuery]long replytoid = -1)
        {
            _ = await CreatePost(new Thread()
            {
                BoardTag = boardtag,
                GeneratePassword = password,
                Text = text,
                ParentId = parent_id,
                Username = username,
                Image = imageurl,
                ReplyToId = replytoid,
                Topic = topic
            });

            return Redirect($"{this.config.Server}/board/{boardtag}");
        }

        [HttpDelete("delete")]
        public ActionResult DeletePostFromBody(int postid = -1, [FromBody]string pass = "")
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

                return NotFound($"Received wrongpass {pass}");
            }
            return NotFound();
        }

        [HttpGet("delete")]
        public ActionResult DeletePostFromQuery([FromQuery]int postid = -1, [FromQuery]string password = "", [FromQuery]string board_tag = "c")
        {
            var res = DeletePostFromBody(postid, password);
            if (res.GetType() == typeof(OkResult)) return this.Redirect($"{this.config.Server}/board/{board_tag}");

            /*return this.View($"board/{board_tag}", new BoardPageModel()
            {
                BoardInfo = this.ctx.Boards.First(x => x.Tag == board_tag),
                Threads = this.ctx.Threads.Where(x => x.BoardTag == board_tag)
            });*/

            else return this.View("Delete", new DeletePageModel()
            {
                PostId = postid,
                Password = password,
                BoardTag = board_tag,
                Failed = true
            });
        }
    }
}
