using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Chandler.Data;
using Chandler.Data.Entities;

namespace Chandler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        private Database database;
        private ServerMeta meta;

        public BoardController(Database database, ServerMeta meta)
        {
            this.database = database;
            this.meta = meta;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Board>> GetBoardList()
        {
            var ctx = database.GetContext();

            var boards = new List<Board>();

            foreach (var p in ctx.Boards)
            {
                boards.Add(p);
            }

            return boards;
        }

        [HttpGet("data")]
        public ActionResult<Board> GetBoardInfo(string tag = "")
        {
            var ctx = database.GetContext();

            if(ctx.Boards.Any(x => x.Tag == tag))
            {
                return ctx.Boards.First(x => x.Tag == tag);
            }
            return this.NotFound("not found");
        }
    }
}
