using Chandler.Data;
using Chandler.Data.Entities;
using Chandler.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chandler.Controllers
{
    /// <summary>
    /// Page Controller
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true), AllowAnonymous]
    public class PageController : Controller
    {
        private readonly Database Database;
        private readonly ServerConfig Config;
        private readonly ThreadController ThreadController;
        private readonly WebhooksController WebhooksController;
        private readonly UserManager<ChandlerUser> UserManager;
        private readonly SignInManager<ChandlerUser> SignInManager;

        private const string INDEX_PAGE_PATH = "/Views/Main/Index.cshtml";
        private const string BOARD_PAGE_PATH = "/Views/Main/Board.cshtml";
        private const int MAX_THREADS_ON_INDEX = 10;

        /// <summary>
        /// Page Ctor
        /// </summary>
        /// <param name="db">Database Model</param>
        /// <param name="config">Server Configuration</param>
        /// <param name="threadcontroller">API Thread Controller</param>
        /// <param name="webhookscontroller">API Webhook Controller</param>
        /// <param name="signIn">Sign in manager</param>
        /// <param name="userManager">User manager</param>
        public PageController(Database db, ServerConfig config, ThreadController threadcontroller, WebhooksController webhookscontroller, SignInManager<ChandlerUser> signIn, UserManager<ChandlerUser> userManager)
        {
            this.Database = db;
            this.Config = config;
            this.ThreadController = threadcontroller;
            this.WebhooksController = webhookscontroller;
            this.SignInManager = signIn;
            this.UserManager = userManager;
        }

        /// <summary>
        /// Main index page
        /// </summary>
        /// <returns>Index Page</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var authres = await HttpContext.AuthenticateAsync("Identity.External");
            if (authres.Succeeded)
            {
                var props = authres.Ticket.Properties;
                var princ = authres.Principal;
                var claims = princ.Claims.ToList();
                var username = claims.First(x => x.Type == ClaimTypes.Name).Value;
                var id = ulong.Parse(claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var email = claims.First(x => x.Type == ClaimTypes.Email).Value;

                await this.SignInManager.SignInWithClaimsAsync(new ChandlerUser()
                {
                    UserName = username,
                    Email = email,
                    DiscordId = id
                }, props, claims);
            }
            else this.Response.Cookies.Delete("Identity.External");
            ViewData["logo"] = this.Config.SiteConfig.SiteLogo;
            return this.View(INDEX_PAGE_PATH, new IndexPageModel()
            {
                Boards = this.Database.Boards,
                Config = this.Config
            });
        }

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
            var threadcount = this.Database.Threads.Where(x => x.BoardTag == tag && x.ParentId < 1).Count();

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
            var threads = this.Database.Threads.Where(x => x.BoardTag == tag && x.ParentId < 1)
                .OrderByDescending(x => this.Database.Threads.Where(a => a.ParentId == x.Id || a.Id == x.Id).Select(b => b.Id).Max())
                .Skip(Math.Abs(p - 1) * MAX_THREADS_ON_INDEX)
                .Take(MAX_THREADS_ON_INDEX)
                .ToList();

            threads.ToList().ForEach(x =>
            {
                x.ChildThreads = this.Database.Threads.Where(a => a.ParentId == x.Id).OrderByDescending(a => a.Id).Take(5);
            });

            var bigOnes = new List<int>();
            foreach (var t in threads)
            {
                if (this.Database.Threads.Where(a => a.ParentId == t.Id).Count() > 5)
                    bigOnes.Add(t.Id);
            }

            return this.View(BOARD_PAGE_PATH, new BoardPageModel()
            {
                BoardInfo = this.Database.Boards.FirstOrDefault(x => x.Tag == tag),
                Threads = threads,
                BigThreads = bigOnes,
                PageCount = pagecount,
                Currentpage = p,
                MaxThreadsPerPage = MAX_THREADS_ON_INDEX,
                Config = this.Config,
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
            var thread = this.Database.Threads.First(x => x.Id == id);
            thread.ChildThreads = this.Database.Threads.Where(a => a.ParentId == thread.Id).OrderBy(x => x.Id);
            var board = this.Database.Boards.First(x => x.Tag == thread.BoardTag);

            return this.View("/Views/Main/Thread.cshtml", new ThreadPageModel()
            {
                BoardInfo = board,
                Thread = thread,
                Config = this.Config
            });
        }

        /// <summary>
        /// New thread page
        /// </summary>
        /// <param name="board">board tag</param>
        /// <param name="parent_id">Id of the parent thread</param>
        /// <param name="replytoid">Id of the thread this post is replying to</param>
        /// <param name="isthreadreply">Whether or not the post is a direct reply to the main thread</param>
        /// <returns>New thread page</returns>
        [Route("new"), HttpGet]
        public IActionResult New([FromQuery]string board, [FromQuery]int parent_id, [FromQuery]long replytoid = -1, [FromQuery]bool isthreadreply = false) =>
            this.View("/Views/Action/New.cshtml", new NewPostPageModel()
            {
                BoardTag = board,
                ParentId = parent_id,
                ReplyToId = replytoid,
                Config = this.Config,
                IsThreadReply = isthreadreply
            });

        /// <summary>
        /// Delete thread page
        /// </summary>
        /// <param name="board_tag">Board tag</param>
        /// <param name="id">Id of the thread</param>
        /// <returns>Board page</returns>
        [Route("delete"), HttpGet]
        public IActionResult Delete([FromQuery]string board_tag, [FromQuery]int id) =>
            this.View("/Views/Action/Delete.cshtml", new DeletePageModel()
            {
                BoardTag = board_tag,
                PostId = id,
                Config = this.Config
            });

        /// <summary>
        /// Webhook page
        /// </summary>
        /// <returns>Webhook page</returns>
        [Route("Webhooks"), HttpGet]
        public IActionResult Webhooks() => this.View("/Views/Main/Webhooks.cshtml", new WebhooksPageModel() { Config = this.Config });

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
            var usr = await this.UserManager.GetUserAsync(this.User);
            var res = await this.ThreadController.CreatePost(new Thread()
            {
                BoardTag = boardtag,
                GeneratePassword = password,
                Text = text,
                ParentId = parent_id,
                Username = username ?? this.User.Identity?.Name,
                Image = imageurl,
                ReplyToId = replytoid,
                Topic = topic,
                UserId = usr?.Id ?? "-1"
            });

            ApiActionStatus apistatus;
            if (res.Result is BadRequestObjectResult badres) apistatus = new ApiActionStatus()
            {
                Message = badres.Value.ToString(),
                ResponseCode = 400,
                Title = "Post"
            };
            else apistatus = new ApiActionStatus()
            {
                Message = "Thread Posted",
                ResponseCode = 200,
                Title = "Post"
            };
            var boardview = (ViewResult)this.Board(boardtag, status: apistatus);
            return this.View(BOARD_PAGE_PATH, boardview.Model);
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
            var res = this.ThreadController.DeletePost(postid, password);
            ApiActionStatus apistatus;
            if (res.Result is BadRequestObjectResult badres)
                apistatus = new ApiActionStatus()
                {
                    Message = badres.Value.ToString(),
                    ResponseCode = 400,
                    Title = "Delete"
                };
            else apistatus = new ApiActionStatus()
            {
                Message = "Thread deleted",
                ResponseCode = 200,
                Title = "Delete"
            };
            var boardview = (ViewResult)this.Board(board_tag, status: apistatus);
            return this.View(BOARD_PAGE_PATH, boardview.Model);
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
            var res = this.WebhooksController.SubscribeWebhook(url, boardtag, threadid); ApiActionStatus apistatus;
            if (res.Result is BadRequestObjectResult badres)
                apistatus = new ApiActionStatus()
                {
                    Message = badres.Value.ToString(),
                    ResponseCode = 400,
                    Title = "Webhook"
                };
            else apistatus = new ApiActionStatus()
            {
                Message = "Success",
                ResponseCode = 200,
                Title = "Webhook"
            };

            return this.View(INDEX_PAGE_PATH, new IndexPageModel()
            {
                ActionStatus = apistatus,
                Boards = this.Database.Boards,
                Config = this.Config
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
            var res = this.WebhooksController.UnSubscribeWebhook(url, boardtag, threadid);
            ApiActionStatus apistatus;
            if (res is BadRequestObjectResult badres)
                apistatus = new ApiActionStatus()
                {
                    Message = badres.Value.ToString(),
                    ResponseCode = 400,
                    Title = "Webhook"
                };
            else apistatus = new ApiActionStatus()
            {
                Message = "Success",
                ResponseCode = 200,
                Title = "Webhook"
            };

            return this.View(INDEX_PAGE_PATH, new IndexPageModel()
            {
                ActionStatus = apistatus,
                Boards = this.Database.Boards,
                Config = this.Config
            });
        }
    }
}
