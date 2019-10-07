using Chandler.Data;
using Chandler.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Chandler.Controllers
{
    [Route("api/[controller]")]
    public class WebhooksController : Controller
    {
        Database Database { get; set; }
        public WebhooksController(Database db) => this.Database = db;

        [HttpGet("subscribe")]
        public ActionResult<WebhookSubscription> SubscribeWebhook([FromQuery]string url, [FromQuery]string password, [FromQuery]string boardtag = null, [FromQuery]int? threadid = null)
        {
            using var ctx = this.Database.GetContext();

            #region You've been bad, go away.
            if (boardtag == null && threadid == null) return this.BadRequest("No board tag or thread id has been provided");
            if (string.IsNullOrEmpty(url)) return this.BadRequest("The provided url was empty");
            if (!new Regex(@"(https:\/\/(?:canary|)\.discordapp\.com\/api\/webhooks\/[\d].+\/[\wzw].+)").IsMatch(url)) return this.BadRequest("The provided url was not a valid discord webhook url");
            if (!ctx.Boards.Any(x => x.Tag == boardtag)) return this.BadRequest("The given board tag doesn't exist");
            if (ctx.WebhookSubscritptions.FirstOrDefault(x => x.Url == url) != null) return this.BadRequest("The given url has already been added");
            #endregion

            var salt = string.Join("", url.Take(new Random().Next(0, url.Length)));
            var hash = Passworder.GenerateHash(password, salt);
            var whs = new WebhookSubscription()
            {
                BoardTag = boardtag,
                ThreadId = threadid,
                Url = url,
                Hash = hash.hash,
                HashCycles = hash.cycles,
                HashSalt = salt,
                UrlId = ulong.Parse(new Regex(@"(\/[\d].+\/)").Match(url).Value.Replace(@"/", ""))
            };
            ctx.WebhookSubscritptions.Add(whs);
            ctx.SaveChanges();

            return whs;
        }

        [HttpGet("unsubscribe")] //, HttpDelete("unsubscribe")]
        public ActionResult<bool> UnSubscribeWebhook([FromQuery]string password, [FromQuery]ulong id)
        {
            if (string.IsNullOrEmpty(password)) return this.BadRequest("Password cannot be empty");
            if (id < 1) return this.BadRequest("The webhook Id is required");
            
            using var ctx = this.Database.GetContext();
            var wh = ctx.WebhookSubscritptions.FirstOrDefault(x => Passworder.CompareHash(password, x.HashSalt, x.Hash, x.HashCycles) && x.UrlId == id);

            if (wh == null && id != 0)
            {
                var mpasswd = ctx.Passwords.First(x => x.Id == -1);
                var passedcheck = Passworder.CompareHash(password, mpasswd.Salt, mpasswd.Hash, mpasswd.Cycles);
                if (passedcheck) wh = ctx.WebhookSubscritptions.FirstOrDefault(x => x.UrlId == id);
            }

            if (wh != null)
            {
                ctx.WebhookSubscritptions.Remove(wh);
                ctx.SaveChanges();
                return true;
            }

            return this.BadRequest("No webhook with the given Id or password could be found");
        }
    }
}
