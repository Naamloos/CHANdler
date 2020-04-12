using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Chandler.Data;
using Chandler.Data.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Chandler.Controllers
{
    /// <summary>
    /// Board Object
    /// </summary>
    [ApiController, Route("api/[controller]"), Produces("application/json"), AllowAnonymous]
    public class BoardController : Controller
    {
        private readonly Database Database;
        
        /// <summary>
        /// Board ctor
        /// </summary>
        /// <param name="database"></param>
        public BoardController(Database database) => this.Database = database;

        /// <summary>
        /// Returns a list of boards
        /// </summary>
        /// <returns>IEnumerable of Board</returns>
        [HttpGet]
        public ActionResult<IEnumerable<Board>> GetBoardList()
        {
            var boards = new List<Board>();
            foreach (var p in this.Database.Boards) boards.Add(p);
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
            if(this.Database.Boards.Any(x => x.Tag == tag)) 
                return this.Database.Boards.First(x => x.Tag == tag);
            else return this.NotFound("Not Found");
        }
    }
}
