using Chandler.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chandler.Data
{
    /// <summary>
    /// Webhook helper for sending posts to a webhook
    /// </summary>
    public static class Webhooker
    {
        /// <summary>
        /// Posts the message to the discord webhook
        /// </summary>
        /// <param name="ctx">Database Context</param>
        /// <param name="thread">The new thread being posted</param>
        /// <returns></returns>
        public static async Task SendContentToAllAsync(DatabaseContext ctx, Thread thread)
        {
            var subs = ctx.WebhookSubscritptions.Where(x => x.BoardTag == thread.BoardTag || x.ThreadId == thread.ParentId);
            using var http = new HttpClient();
            var jsondata = JsonConvert.SerializeObject(new DiscordWebhookBody()
            {
                Content = $"**{thread.Username}** Posted:\n__{thread.Topic ?? "No Topic Set"}__\n{thread.Text}"
            });
            foreach (var sub in subs)
            {
                var res = await http.PostAsync($"https://discordapp.com/api/webhooks/{sub.WebhookId}/{sub.Token}", new StringContent(jsondata, Encoding.UTF8, "application/json")).ConfigureAwait(false);
                if (!res.IsSuccessStatusCode)
                {
                    var cont = await res.Content.ReadAsStringAsync();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error when sending data to webhook: {cont}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        /// <summary>
        /// Posts the message to the discord webhook
        /// </summary>
        /// <param name="sub">The webhook subscription to use</param>
        /// <param name="body">Full post body object</param>
        /// <returns></returns>
        public static async Task<bool> SendBodyAsync(WebhookSubscription sub, DiscordWebhookBody body)
        {
            using var http = new HttpClient();
            var jsondata = JsonConvert.SerializeObject(body);
            var res = await http.PostAsync($"https://discordapp.com/api/webhooks/{sub.WebhookId}/{sub.Token}", new StringContent(jsondata, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            var cont = await res.Content.ReadAsStringAsync();
            return res.IsSuccessStatusCode;
        }

        /// <summary>
        /// Posts the embed and content to the discord webhook
        /// </summary>
        /// <param name="sub">The webhook subscription to use</param>
        /// <param name="embed">The embed to post</param>
        /// <returns></returns>
        public static async Task<bool> SendEmbedAsync(WebhookSubscription sub, Embed embed)
            => await SendBodyAsync(sub, new DiscordWebhookBody()
            {
                Content = "",
                Embed = embed
            });
    }
}
