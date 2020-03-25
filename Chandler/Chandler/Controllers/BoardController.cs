using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Chandler.Data;
using Chandler.Data.Entities;

namespace Chandler.Controllers
{
    /// <summary>
    /// Board Object
    /// </summary>
    [ApiController, Route("api/[controller]"), Produces("application/json")]
    public class BoardController : ControllerBase
    {
        private readonly Database database;
        private readonly ServerMeta meta;
        
        /// <summary>
        /// Board ctor
        /// </summary>
        /// <param name="database"></param>
        /// <param name="meta"></param>
        public BoardController(Database database, ServerMeta meta)
        {
            this.database = database;
            this.meta = meta;
        }

        /// <summary>
        /// Returns a list of boards
        /// </summary>
        /// <returns>IEnumerable of Board</returns>
        [HttpGet]
        public ActionResult<IEnumerable<Board>> GetBoardList()
        {
            using var ctx = database.GetContext();

            var boards = new List<Board>();

            foreach (var p in ctx.Boards) boards.Add(p);

            return boards;
        }

        /// <summary>
        /// Returns data on a board
        /// </summary>
        /// <param name="tag">The tag of the board</param>
        /// <returns>Board</returns>
        [HttpGet("data")]
        public ActionResult<Board> GetBoardInfo([FromQuery]string tag)
        {
            using var ctx = database.GetContext();

            if(ctx.Boards.Any(x => x.Tag == tag)) return ctx.Boards.First(x => x.Tag == tag);

            return this.NotFound("Not Found");
        }
    }
}
