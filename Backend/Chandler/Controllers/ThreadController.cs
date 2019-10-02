using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Chandler.Data;
using Chandler.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Chandler.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class ThreadController : ControllerBase
    {
        private delegate Task PostCreatedEvent(Thread thread, DiscordWebhookBody body);
        private event PostCreatedEvent PostCreated;

        private readonly Database database;
        private readonly ServerMeta meta;

        public ThreadController(Database database, ServerMeta meta)
        {
            this.database = database;
            this.meta = meta;
            this.PostCreated += this.ThreadController_PostCreated;
        }

        private async Task ThreadController_PostCreated(Thread thread, DiscordWebhookBody body)
        {
            using var http = new HttpClient();
            using var ctx = this.database.GetContext();
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
            using var ctx = database.GetContext();

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
        public ActionResult<IEnumerable<Thread>> GetPosts(int id = -1)
        {
            using var ctx = database.GetContext();

            return ctx.Threads.Where(x => x.ParentId == id).OrderBy(x => x.Id).ToList();
        }

        [HttpPost("create")]
        public async Task<ActionResult<Thread>> CreatePost([FromBody] Thread newpost)
        {
            using var ctx = database.GetContext();

            newpost.Id = 0;

            if (string.IsNullOrEmpty(newpost.Text))
                return BadRequest("Text too short");

            if (string.IsNullOrEmpty(newpost.Username))
                newpost.Username = "Anonymous";

            var passw = newpost.generatepass;
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

            var encoder = HtmlEncoder.Default;
            newpost.Text = encoder.Encode(newpost.Text);
            newpost.Image = encoder.Encode(newpost.Image);
            newpost.Username = encoder.Encode(newpost.Username);
            newpost.Topic = encoder.Encode(newpost.Topic);

            newpost.generatepass = "";
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

        [HttpDelete("delete")]
        public ActionResult DeletePost(int postid = -1, [FromBody]string pass = "")
        {
            using var ctx = database.GetContext();

            if (ctx.Threads.Any(x => x.Id == postid))
            {
                var thread = ctx.Threads.First(x => x.Id == postid);
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
    }
}
