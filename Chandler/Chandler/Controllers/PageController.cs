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
    /// <summary>
    /// Page Controller
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PageController : Controller
    {
        private readonly HttpClient http;
        private readonly Database database;
        private readonly ServerConfig config;
        private readonly DatabaseContext ctx;

        /// <summary>
        /// Page Ctor
        /// </summary>
        /// <param name="db">Database Model</param>
        /// <param name="config">Server Configuration</param>
        public PageController(Database db, ServerConfig config)
        {
            this.database = db;
            this.config = config;
            this.http = new HttpClient();
            this.ctx = this.database.GetContext();
            this.ctx.Database.EnsureCreated();
        }

        /// <summary>
        /// Main index page
        /// </summary>
        /// <returns>Index Page</returns>
        [HttpGet]
        public IActionResult Index() => this.View(new IndexPageModel()
        {
            Boards = ctx.Boards,
            Config = this.config
        });

        /// <summary>
        /// Board page
        /// </summary>
        /// <param name="tag">Board Tag</param>
        /// <returns>Board page for the given board tag</returns>
        [Route("board/{tag}"), HttpGet]
        public IActionResult Board(string tag)
        {
            var threads = this.ctx.Threads.Where(x => x.BoardTag == tag);
            threads.ToList().ForEach(x => x.ChildThreads = this.ctx.Threads.Where(a => a.ParentId == x.Id));
            return this.View(new BoardPageModel()
            {
                BoardInfo = this.ctx.Boards.FirstOrDefault(x => x.Tag == tag),
                Threads = threads
            });
        }

        /// <summary>
        /// New thread page
        /// </summary>
        /// <param name="board">board tag</param>
        /// <param name="parent_id">Id of the parent thread</param>
        /// <param name="replytoid">Id of the thread this post is replying to</param>
        /// <returns>New thread page</returns>
        [Route("new"), HttpGet]
        public IActionResult New([FromQuery]string board, [FromQuery]int parent_id, [FromQuery]long replytoid = -1) =>
            this.View(new NewPostPageModel()
            {
                BoardTag = board,
                ParentId = parent_id,
                ReplyToId = replytoid
            });

        /// <summary>
        /// Delete thread page
        /// </summary>
        /// <param name="board_tag">Board tag</param>
        /// <param name="id">Id of the thread</param>
        /// <returns>Board page</returns>
        [Route("delete"), HttpGet]
        public IActionResult Delete([FromQuery]string board_tag, [FromQuery]int id) =>
            this.View(new DeletePageModel()
            {
                BoardTag = board_tag,
                PostId = id
            });

        /// <summary>
        /// Webhook page
        /// </summary>
        /// <returns>Webhook page</returns>
        [Route("Webhooks"), HttpGet]
        public IActionResult Webhooks() => this.View();

        /// <summary>
        /// Creates a new thread and returns to the board page
        /// </summary>
        /// <param name="boardtag">Board tag</param>
        /// <param name="text">Message text</param>
        /// <param name="parent_id">Parent Id of the thread</param>
        /// <param name="username">Username to display</param>
        /// <param name="topic">Topic of the thread</param>
        /// <param name="password">Password to delete the post</param>
        /// <param name="imageurl">Url for the post's image</param>
        /// <param name="replytoid">Id of the post this post is replying to</param>
        /// <returns>Board page</returns>
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

        /// <summary>
        /// Deletes a post using a query
        /// </summary>
        /// <param name="postid">The id of the post to delete</param>
        /// <param name="password">Password to delete the post</param>
        /// <param name="board_tag">Board tag</param>
        /// <returns>Board page</returns>
        [Route("thread/deletepost"), HttpGet]
        public async Task<IActionResult> DeletePostFromQuery([FromQuery]int postid, [FromQuery]string password, [FromQuery]string board_tag)
        {
            var res = await http.DeleteAsync(new Uri($"{this.config.Server}/api/thread/delete?postid={postid}&pass={password}"));
            return this.Redirect($"{this.config.Server}/board/{board_tag}");
        }

        /// <summary>
        /// Creates a new webhook link and returns to the base server address
        /// </summary>
        /// <param name="url">Webhook URL</param>
        /// <param name="password">Password to delete the webhook</param>
        /// <param name="boardtag">Board tag to listen to </param>
        /// <param name="threadid">Thread Id to listen to</param>
        /// <returns>Main Index Page</returns>
        [Route("formsub"), HttpGet]
        public async Task<IActionResult> FormSub([FromQuery]string url, [FromQuery]string password, [FromQuery]string boardtag = null, [FromQuery]int? threadid = null)
        {
            var res = await http.GetAsync(new Uri($"{this.config.Server}/api/webhooks/subscribe?url={url}&password={password}&boardtag={boardtag}&threadid={threadid}"));
            return Redirect("/");
        }

        /// <summary>
        /// Deletes a webhook link and returns to the base server address
        /// </summary>
        /// <param name="password">Password to delete webhook</param>
        /// <param name="id">Id of the webhook</param>
        /// <returns>Main Index Page</returns>
        [Route("formunsub"), HttpGet]
        public async Task<IActionResult> FormUnsub([FromQuery]string password, [FromQuery]ulong id)
        {
            var res = await http.GetAsync(new Uri($"{this.config.Server}/api/webhooks/unsubscribe?password={password}&id={id}"));
            return Redirect("/");
        }
    }
}
