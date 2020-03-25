#pragma warning disable CS1591

using Chandler.Data;
using Chandler.Data.Entities;
using Chandler.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chandler.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PageController : Controller
    {
        private readonly HttpClient http;
        private readonly Database database;
        private readonly ServerConfig config;
        private readonly DatabaseContext ctx;

        public PageController(Database db, ServerConfig config)
        {
            this.database = db;
            this.config = config;
            this.http = new HttpClient();
            this.ctx = this.database.GetContext();
            this.ctx.Database.EnsureCreated();
        }

        [HttpGet]
        public IActionResult Index() => this.View(new IndexPageModel()
        {
            Boards = ctx.Boards,
            Config = this.config
        });

        [Route("board/{tag}"), HttpGet]
        public IActionResult Board(string tag) => this.View(new BoardPageModel()
        {
            BoardInfo = this.ctx.Boards.FirstOrDefault(x => x.Tag == tag),
            Threads = this.ctx.Threads.Where(x => x.BoardTag == tag)
        });

        [Route("new"), HttpGet]
        public IActionResult New([FromQuery]string board, [FromQuery]int parent_id, [FromQuery]long replytoid = -1) =>
            this.View(new NewPostPageModel()
            {
                BoardTag = board,
                ParentId = parent_id,
                ReplyToId = replytoid
            });

        [Route("delete"), HttpGet]
        public IActionResult Delete([FromQuery]string board_tag, [FromQuery]int id) =>
            this.View(new DeletePageModel()
            {
                BoardTag = board_tag,
                PostId = id
            });

        [Route("Webhooks"), HttpGet]
        public IActionResult Webhooks() => this.View();

        [Route("thread/create"), HttpGet]
        public async Task<IActionResult> CreatePostAndRedirect([FromQuery]string boardtag, [FromQuery]string text, [FromQuery]int parent_id = -1, [FromQuery]string username = null, [FromQuery]string topic = null, [FromQuery]string password = null, [FromQuery]string imageurl = null, [FromQuery]long replytoid = -1)
        {
            var res = await http.PostAsync(new Uri($"{this.config.Server}/api/thread/create"), new StringContent(JsonConvert.SerializeObject(new Thread()
            {
                BoardTag = boardtag,
                GeneratePassword = password,
                Text = text,
                ParentId = parent_id,
                Username = username,
                Image = imageurl,
                ReplyToId = replytoid,
                Topic = topic
            }), Encoding.UTF8, "application/json"));

            return Redirect($"{this.config.Server}/board/{boardtag}");
        }

        [Route("thread/deletepost"), HttpGet]
        public async Task<IActionResult> DeletePostFromQuery([FromQuery]int postid = -1, [FromQuery]string password = "", [FromQuery]string board_tag = "c")
        {
            var res = await http.DeleteAsync(new Uri($"{this.config.Server}/api/thread/delete?postid={postid}&pass={password}"));
            return this.Redirect($"{this.config.Server}/board/{board_tag}");
        }

        [Route("formsub"), HttpGet]
        public async Task<IActionResult> FormSub([FromQuery]string url, [FromQuery]string password, [FromQuery]string boardtag = null, [FromQuery]int? threadid = null)
        {
            var res = await http.GetAsync(new Uri($"{this.config.Server}/api/webhooks/subscribe?url={url}&password={password}&boardtag={boardtag}&threadid={threadid}"));
            return Redirect("/");
        }

        [Route("formunsub"), HttpGet]
        public async Task<IActionResult> FormUnsub([FromQuery]string password, [FromQuery]ulong id)
        {
            var res = await http.GetAsync(new Uri($"{this.config.Server}/api/webhooks/unsubscribe?password={password}&id={id}"));
            return Redirect("/");
        }
    }
}
