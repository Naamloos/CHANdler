﻿using Chandler.Data;
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
    [Route("api/[controller]"), Produces("application/json")]
    public class WebhooksController : Controller
    {
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
        /// Subscribes a webhook to the given thread or board
        /// </summary>
        /// <param name="url">The webhook's URL</param>
        /// <param name="password">Password used when removing the webhook</param>
        /// <param name="boardtag">The tag of the board to listen to</param>
        /// <param name="threadid">The ID of the thread to listen to</param>
        /// <returns>WebhookSubscription Object</returns>
        [HttpGet("subscribe")]
        public async Task<ActionResult<WebhookSubscription>> SubscribeWebhook([FromQuery]string url, [FromQuery]string password, [FromQuery]string boardtag = null, [FromQuery]int? threadid = null)
        {
            using var ctx = this.Database.GetContext();

            #region You've been bad, go away.
            if (boardtag == null && threadid == null) return this.BadRequest("No board tag or thread id has been provided");
            if (string.IsNullOrEmpty(url)) return this.BadRequest("The provided url was empty");
            if (!new Regex(@"(https:\/\/(?:canary|)\.discordapp\.com\/api\/webhooks\/[\d].+\/[\wzw].+)").IsMatch(url)) return this.BadRequest("The provided url was not a valid discord webhook url");
            if (!ctx.Boards.Any(x => x.Tag == boardtag)) return this.BadRequest("The given board tag doesn't exist");
            if (ctx.WebhookSubscritptions.FirstOrDefault(x => x.Url == url) != null) return this.BadRequest("The given url has already been added");
            #endregion

            var hash = Passworder.GenerateHash(password, this.Config.DefaultPassword);
            var pw = new Password()
            {
                Hash = hash.Hash,
                Salt = hash.Salt
            };

            this.Database.GetContext().Passwords.Add(pw);
            await this.Database.GetContext().SaveChangesAsync();

            var whs = new WebhookSubscription()
            {
                BoardTag = boardtag,
                ThreadId = threadid,
                Url = url,
                PasswordId = pw.Id,
                UrlId = ulong.Parse(new Regex(@"(\/[\d].+\/)").Match(url).Value.Replace(@"/", ""))
            };
            ctx.WebhookSubscritptions.Add(whs);
            ctx.SaveChanges();

            return whs;
        }

        /// <summary>
        /// Ubsubscribes a webhook
        /// </summary>
        /// <param name="passwordid">The ID of the password</param>
        /// <param name="password">The password</param>
        /// <param name="id">Webhook's ID</param>
        /// <returns>True on success</returns>
        /// <response code="400">When ID is invalid or password is incorrect</response>
        [HttpDelete("unsubscribe")]
        public ActionResult<bool> UnSubscribeWebhook([FromQuery]int passwordid, [FromQuery]string password, [FromQuery]ulong id)
        {
            if (id < 1) return this.BadRequest("The webhook ID is required");
            
            using var ctx = this.Database.GetContext();
            var pw = ctx.Passwords.FirstOrDefault(x => x.Id == passwordid);
            var validpass = Passworder.VerifyPassword(password, pw.Hash, pw.Salt, this.Config.DefaultPassword);
            var wh = ctx.WebhookSubscritptions.FirstOrDefault(x => x.UrlId == id);

            if (wh == null && id != 0 && !validpass)
            {
                var mpasswd = ctx.Passwords.First(x => x.Id == -1);
                var passedcheck = Passworder.VerifyPassword(password, mpasswd.Hash, mpasswd.Salt, this.Config.DefaultPassword);
                if (passedcheck) wh = ctx.WebhookSubscritptions.FirstOrDefault(x => x.UrlId == id);
                ctx.WebhookSubscritptions.Remove(wh);
                ctx.SaveChanges();
                return true;
            }

            if (wh != null && validpass)
            {
                ctx.WebhookSubscritptions.Remove(wh);
                ctx.SaveChanges();
                return true;
            }

            return this.BadRequest("No webhook with the given ID or password could be found");
        }
    }
}
