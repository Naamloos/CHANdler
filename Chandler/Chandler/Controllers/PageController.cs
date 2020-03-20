using Chandler.Data;
using Chandler.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Http;

namespace Chandler.Controllers
{
    public class PageController : Controller
    {
        private readonly Database database;
        private readonly ServerConfig conf;
        private readonly DatabaseContext ctx;

        public PageController(Database db, ServerConfig config)
        {
            this.database = db;
            this.conf = config;
            this.ctx = this.database.GetContext();
            this.ctx.Database.EnsureCreated();
        }

        public IActionResult Index() => this.View(new IndexPageModel()
        {
            Boards = ctx.Boards,
            Config = this.conf
        });

        [Route("board/{tag}")]
        public IActionResult Board(string tag) => this.View(new BoardPageModel()
        {
            BoardInfo = this.ctx.Boards.FirstOrDefault(x => x.Tag == tag),
            Threads = this.ctx.Threads.Where(x => x.BoardTag == tag)
        });

        [Route("new")]
        public IActionResult New([FromQuery]string board, [FromQuery]int parent_id, [FromQuery]long replytoid = -1) =>
            this.View(new NewPostPageModel()
            {
                BoardTag = board,
                ParentId = parent_id,
                ReplyToId = replytoid
            });

        [Route("delete")]
        public IActionResult Delete([FromQuery]string board_tag, [FromQuery]int id) =>
            this.View(new DeletePageModel()
            {
                BoardTag = board_tag,
                PostId = id
            });

        [Route("Webhooks")]
        public IActionResult Webhooks() => this.View();
    }
}
