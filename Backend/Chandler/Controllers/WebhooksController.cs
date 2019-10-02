using Chandler.Data;
using Chandler.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.RegularExpressions;

namespace Chandler.Controllers
{
    [Route("[controller]")]
    public class WebhooksController : Controller
    {
        Database Database { get; set; }
        public WebhooksController(Database db) => this.Database = db;

        [HttpGet("subscribe")]
        public ActionResult<WebhookSubscription> SubscribeWebhook([FromQuery]string url, [FromQuery]string secret, [FromQuery]string boardtag = null, [FromQuery]int? threadid = null)
        {
            if (boardtag == null && threadid == null) return this.BadRequest("No board tag or thread id has been provided");
            if (string.IsNullOrEmpty(url)) return this.BadRequest("The provided url was empty");
            if (!new Regex(@"(https:\/\/(?:canary|)\.discordapp\.com\/api\/webhooks\/[\d].+\/[\wzw].+)").IsMatch(url)) return this.BadRequest("The provided url was not a valid discord webhook url");

            using var ctx = this.Database.GetContext();
            if (ctx.WebhookSubscritptions.FirstOrDefault(x => x.Url == url) != null) return this.BadRequest("The given url has already been added");

            var hash = Passworder.GenerateHash(url, secret);
            var whs = new WebhookSubscription()
            {
                BoardTag = boardtag,
                ThreadId = threadid,
                Url = url,
                HashSecret = hash.hash,
                HashCycles = hash.cycles
            };
            ctx.WebhookSubscritptions.Add(whs);
            ctx.SaveChanges();

            return whs;
        }

        [HttpDelete("unsubscribe")]
        public ActionResult<bool> UnSubscribeWebhook([FromQuery]string hash)
        {
            using var ctx = this.Database.GetContext();
            var sub = ctx.WebhookSubscritptions.FirstOrDefault(x => x.HashSecret == hash);
            if (sub != null)
            {
                ctx.WebhookSubscritptions.Remove(sub);
                return true;
            }
            else return false;
        }
    }
}
