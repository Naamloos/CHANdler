using Chandler.Data;
using Chandler.Data.Entities;
using Chandler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private readonly Database database;
        private readonly ServerConfig config;
        private readonly DatabaseContext ctx;
        private readonly ThreadController threadcontroller;
        private readonly WebhooksController webhookscontroller;
        private const int MAX_THREADS_ON_INDEX = 10;

        /// <summary>
        /// Page Ctor
        /// </summary>
        /// <param name="db">Database Model</param>
        /// <param name="config">Server Configuration</param>
        /// <param name="threadcontroller">API Thread Controller</param>
        /// <param name="webhookscontroller">API Webhook Controller</param>
        public PageController(Database db, ServerConfig config, ThreadController threadcontroller, WebhooksController webhookscontroller)
        {
            this.database = db;
            this.config = config;
            this.ctx = this.database.GetContext();
            this.ctx.Database.EnsureCreated();
            this.threadcontroller = threadcontroller;
            this.webhookscontroller = webhookscontroller;
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
        /// <param name="p">Page to load</param>
        /// <param name="status">Api Action status to return with view</param>
        /// <returns>Board page for the given board tag</returns>
        [Route("board/{tag}"), HttpGet]
        public IActionResult Board(string tag, [FromQuery]int p = 1, ApiActionStatus status = null)
        {
            var threadcount = this.ctx.Threads.Where(x => x.BoardTag == tag && x.ParentId < 1).Count();

            var pagecount = 1;
            // calculating page count
            var remainder = threadcount % MAX_THREADS_ON_INDEX;
            if (threadcount - remainder >= MAX_THREADS_ON_INDEX) // we can only have more than 1 page when we have over 10 threads
            {
                pagecount = ((threadcount - remainder) / MAX_THREADS_ON_INDEX);
                if (remainder > 0)
                    pagecount++;
            }

            // Get threads in order of last bumped.
            var threads = this.ctx.Threads.Where(x => x.BoardTag == tag && x.ParentId < 1)
                .OrderByDescending(x => this.ctx.Threads.Where(a => a.ParentId == x.Id || a.Id == x.Id).Select(b => b.Id).Max())
                .Skip(Math.Abs(p - 1) * MAX_THREADS_ON_INDEX)
                .Take(MAX_THREADS_ON_INDEX)
                .ToList();

            threads.ToList().ForEach(x =>
            {
                x.ChildThreads = this.ctx.Threads.Where(a => a.ParentId == x.Id).OrderByDescending(a => a.Id).Take(5);
            });

            var bigOnes = new List<int>();
            foreach (var t in threads)
            {
                if (this.ctx.Threads.Where(a => a.ParentId == t.Id).Count() > 5)
                    bigOnes.Add(t.Id);
            }

            return this.View(new BoardPageModel()
            {
                BoardInfo = this.ctx.Boards.FirstOrDefault(x => x.Tag == tag),
                Threads = threads,
                BigThreads = bigOnes,
                PageCount = pagecount,
                Currentpage = p,
                MaxThreadsPerPage = MAX_THREADS_ON_INDEX,
                Config = this.config,
                ActionStatus = status
            });
        }

        /// <summary>
        /// Thread page
        /// </summary>
        /// <param name="id">Thread id</param>
        /// <returns>Thread page for the given thread id</returns>
        [Route("thread/{id}"), HttpGet]
        public IActionResult Thread(int id)
        {
            var thread = this.ctx.Threads.First(x => x.Id == id);
            thread.ChildThreads = this.ctx.Threads.Where(a => a.ParentId == thread.Id).OrderBy(x => x.Id);
            var board = this.ctx.Boards.First(x => x.Tag == thread.BoardTag);

            return this.View(new ThreadPageModel()
            {
                BoardInfo = board,
                Thread = thread,
                Config = this.config
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
                ReplyToId = replytoid,
                Config = this.config
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
                PostId = id,
                Config = this.config
            });

        /// <summary>
        /// Webhook page
        /// </summary>
        /// <returns>Webhook page</returns>
        [Route("Webhooks"), HttpGet]
        public IActionResult Webhooks() => this.View(new WebhooksPageModel() { Config = this.config });

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
        public async Task<IActionResult> Board([FromQuery]string boardtag, [FromQuery]string text, [FromQuery]int parent_id = -1, [FromQuery]string username = null, [FromQuery]string topic = null, [FromQuery]string password = null, [FromQuery]string imageurl = null, [FromQuery]long replytoid = -1)
        {
            var response = await threadcontroller.CreatePost(new Thread()
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
            var badres = response.Result as BadRequestObjectResult;
            var boardview = (ViewResult)this.Board(boardtag, status: new ApiActionStatus()
            {
                Message = (badres == null) ? "Thread posted" : badres.Value.ToString(),
                ResponseCode = (badres == null) ? 200 : 400,
                Title = "Post"
            });
            return this.View("Board", boardview.Model);
        }

        /// <summary>
        /// Deletes a post using a query
        /// </summary>
        /// <param name="postid">The id of the post to delete</param>
        /// <param name="password">Password to delete the post</param>
        /// <param name="board_tag">Board tag</param>
        /// <returns>Board page</returns>
        [Route("thread/deletepost"), HttpGet]
        public IActionResult DeletePostFromQuery([FromQuery]int postid, [FromQuery]string password, [FromQuery]string board_tag)
        {
            var res = threadcontroller.DeletePost(postid, password);
            var badres = res.Result as BadRequestObjectResult;
            var boardview = (ViewResult)this.Board(board_tag, status: new ApiActionStatus()
            {
                Message = (badres == null) ? "Thread deleted" : badres.Value.ToString(),
                ResponseCode = (badres == null) ? 200 : 400,
                Title = "Delete"
            });
            return this.View("Board", boardview.Model);
        }

        /// <summary>
        /// Creates a new webhook link and returns to the base server address
        /// </summary>
        /// <param name="url">Webhook URL</param>
        /// <param name="boardtag">Board tag to listen to </param>
        /// <param name="threadid">Thread Id to listen to</param>
        /// <returns>Main Index Page</returns>
        [Route("webhooksub"), HttpGet]
        public IActionResult FormSub([FromQuery]string url, [FromQuery]string boardtag = null, [FromQuery]int? threadid = null)
        {
            var res = webhookscontroller.SubscribeWebhook(url, boardtag, threadid);
            var badres = res.Result as BadRequestObjectResult;
            return this.View("Index", new IndexPageModel()
            {
                ActionStatus = new ApiActionStatus()
                {
                    Message = (badres != null) ? badres.Value.ToString() : "Success",
                    ResponseCode = (res.Result as OkObjectResult) == null ? 400 : 200,
                    Title = "Webhook"
                },
                Boards = this.ctx.Boards,
                Config = this.config
            });
        }

        /// <summary>
        /// Deletes a webhook link and returns to the base server address
        /// </summary>
        /// <param name="url">URL of the webhook</param>
        /// <param name="boardtag">Tag of the board</param>
        /// <param name="threadid">ID of the thread</param>
        /// <returns>Main Index Page</returns>
        [Route("webhookunsub"), HttpGet]
        public IActionResult FormUnsub([FromQuery]string url, [FromQuery]string boardtag = null, [FromQuery]int? threadid = null)
        {
            var res = webhookscontroller.UnSubscribeWebhook(url, boardtag, threadid);
            var badres = res as BadRequestObjectResult;
            return this.View("Index", new IndexPageModel()
            {
                ActionStatus = new ApiActionStatus()
                {
                    Message = (badres != null) ? badres.Value.ToString() : "Success",
                    ResponseCode = (badres != null) ? 400 : 200,
                    Title = "Webhook"
                },
                Boards = this.ctx.Boards,
                Config = this.config
            });
        }
    }
}
