using Chandler.Data;
using Chandler.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.RegularExpressions;

namespace Chandler.Controllers
{
    /// <summary>
    /// Webhook controller
    /// </summary>
    [ApiController, Route("api/[controller]"), Produces("application/json"), AllowAnonymous, BeforeExecute]
    public class WebhooksController : Controller
    {
        const string WebhookRegex = @"(https:\/\/(?:canary\.|)discordapp\.com\/api\/webhooks\/([0-9]+)\/(.+))";
        Database Database { get; set; }

        /// <summary>
        /// Webhook Ctor
        /// </summary>
        /// <param name="db"></param>
        public WebhooksController(Database db) => this.Database = db;

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
        /// <response code="400">When no board tag or thread id has been sent, the url is empty, the url is in valid, the board/thread doesnt exist, or when the webhook has already been assigned to the board/thread</response>
        [HttpGet("subscribe")]
        public ActionResult<WebhookSubscription> SubscribeWebhook([FromQuery]string url, [FromQuery]string boardtag = null, [FromQuery]int? threadid = null)
        {
            #region You've been bad, go away.
            if (boardtag == null && threadid == null) return this.BadRequest("No board tag or thread id has been provided");
            if (string.IsNullOrEmpty(url)) return this.BadRequest("The provided url was empty");
            if (!new Regex(WebhookRegex).IsMatch(url)) return this.BadRequest("The provided url was not a valid discord webhook url");
            if (!this.Database.Boards.Any(x => x.Tag == boardtag) && threadid == null) return this.BadRequest("The given board tag doesn't exist");
            if (!this.Database.Threads.Any(x => x.Id == threadid) && boardtag == null) return this.BadRequest("The given thread id doesn't exist");
            #endregion

            (var whid, var whtoken) = ParseWebhook(url);
            var wh = this.Database.WebhookSubscritptions.FirstOrDefault(x => x.Token == whtoken && x.WebhookId == whid);
            if (wh != null)
            {
                if (wh.BoardTag != null && wh.BoardTag == boardtag)
                    return this.BadRequest("The webhook url has already been assigned to this board");

                else if (wh.ThreadId > 0 && wh.ThreadId == threadid)
                    return this.BadRequest("The webhook url has already been assigned to this thread");
            }

            var whs = new WebhookSubscription()
            {
                BoardTag = boardtag,
                ThreadId = threadid,
                Token = whtoken,
                WebhookId = whid
            };

            this.Database.WebhookSubscritptions.Add(whs);
            this.Database.SaveChanges();

            return whs;
        }

        /// <summary>
        /// Ubsubscribes a webhook
        /// </summary>
        /// <param name="url">Webhook's URL</param>
        /// <param name="boardtag">The tag of the board to unsub from</param>
        /// <param name="threadid">The Id of the thread to unsub from</param>
        /// <returns>200 OK on success</returns>
        /// <response code="400">When ID is invalid or password is incorrect</response>
        [HttpDelete("unsubscribe")]
        public IActionResult UnSubscribeWebhook([FromQuery]string url, [FromQuery]string boardtag = null, [FromQuery]int? threadid = null)
        {
            if (boardtag == null && threadid == null)
                return this.BadRequest("No board tag or thread id was given");

            (var whid, var whtoken) = ParseWebhook(url);

            if (!this.Database.WebhookSubscritptions.Any(x => x.Token == whtoken && x.WebhookId == whid))
                return this.BadRequest("The given url has not been added");

            var matches = this.Database.WebhookSubscritptions.Where(x => (x.Token == whtoken && x.WebhookId == whid) && (x.BoardTag == boardtag || x.ThreadId == threadid));
            this.Database.WebhookSubscritptions.RemoveRange(matches);
            this.Database.SaveChanges();
            return this.Ok();
        }
    }
}
