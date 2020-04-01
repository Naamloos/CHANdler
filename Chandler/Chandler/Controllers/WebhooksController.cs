using Chandler.Data;
using Chandler.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chandler.Controllers
{
    /// <summary>
    /// Webhook controller
    /// </summary>
    [ApiController, Route("api/[controller]"), Produces("application/json")]
    public class WebhooksController : Controller
    {
        const string WebhookRegex = @"(https:\/\/(?:canary\.|)discordapp\.com\/api\/webhooks\/([0-9]+)\/(.+))";
        ServerConfig Config { get; set; }
        Database Database { get; set; }

        /// <summary>
        /// Webhook Ctor
        /// </summary>
        /// <param name="db"></param>
        /// <param name="config"></param>
        public WebhooksController(Database db, ServerConfig config)
        {
            this.Config = config;
            this.Database = db;
        }

        /// <summary>
        /// Parses a webhook url into its Id and Token
        /// </summary>
        /// <param name="url">The webhook url you wish to parse</param>
        /// <returns>Id and Token as a tuple</returns>
        private (ulong Id, string Token) ParseWebhook(string url)
        {
            var match = Regex.Match(url, WebhookRegex);
            var id = match.Groups[2].Value;
            var token = match.Groups[3].Value;
            return (ulong.Parse(id), token);
        }

        /// <summary>
        /// Subscribes a webhook to the given thread or board
        /// </summary>
        /// <param name="url">The webhook's URL</param>
        /// <param name="boardtag">The tag of the board to listen to</param>
        /// <param name="threadid">The ID of the thread to listen to</param>
        /// <returns>WebhookSubscription Object</returns>
        [HttpGet("subscribe")]
        public ActionResult<WebhookSubscription> SubscribeWebhook([FromQuery]string url, [FromQuery]string boardtag = null, [FromQuery]int? threadid = null)
        {
            using var ctx = this.Database.GetContext();

            #region You've been bad, go away.
            if (boardtag == null && threadid == null) return this.BadRequest("No board tag or thread id has been provided");
            if (string.IsNullOrEmpty(url)) return this.BadRequest("The provided url was empty");
            if (!new Regex(WebhookRegex).IsMatch(url)) return this.BadRequest("The provided url was not a valid discord webhook url");
            if (!ctx.Boards.Any(x => x.Tag == boardtag)) return this.BadRequest("The given board tag doesn't exist");
            #endregion

            var wbhk = ParseWebhook(url);
            if (ctx.WebhookSubscritptions.Any(x => x.Token == wbhk.Token && x.WebhookId == wbhk.Id))
                return this.BadRequest("The given url has already been added");

            var whs = new WebhookSubscription()
            {
                BoardTag = boardtag,
                ThreadId = threadid,
                Token = wbhk.Item2,
                WebhookId = wbhk.Item1
            };

            ctx.WebhookSubscritptions.Add(whs);
            ctx.SaveChanges();

            return whs;
        }

        /// <summary>
        /// Ubsubscribes a webhook
        /// </summary>
        /// <param name="url">Webhook's URL</param>
        /// <returns>True on success</returns>
        /// <response code="400">When ID is invalid or password is incorrect</response>
        [HttpDelete("unsubscribe")]
        public ActionResult<bool> UnSubscribeWebhook([FromQuery]string url)
        {
            var whp = ParseWebhook(url);
            
            using var ctx = this.Database.GetContext();
            if (!ctx.WebhookSubscritptions.Any(x => x.Token == whp.Token && x.WebhookId == whp.Id))
                return this.BadRequest("The given url has not been added");

            var matches = ctx.WebhookSubscritptions.Where(x => x.WebhookId == whp.Id && x.Token == whp.Token);
            ctx.WebhookSubscritptions.RemoveRange(matches);
            ctx.SaveChanges();
            return true;
        }
    }
}
