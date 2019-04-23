using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chandler.Data;
using Chandler.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chandler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThreadController : ControllerBase
    {
        private Database database;
        private ServerMeta meta;

        public ThreadController(Database database, ServerMeta meta)
        {
            this.database = database;
            this.meta = meta;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Thread>> GetThreads(string tag = "")
        {
            var ctx = database.GetContext();

            if (ctx.Threads.Any(x => x.BoardTag == tag && x.ParentId == -1))
            {
                return ctx.Threads.Where(x => x.BoardTag == tag && x.ParentId == -1)
                    .OrderByDescending(x => ctx.Threads.Where(y => y.ParentId == x.Id || y.Id == x.Id).Select(y => y.Id).Max())
                    .ToList();
            }
            return this.NotFound("not found");
        }

        [HttpGet("post")]
        public ActionResult<IEnumerable<Thread>> GetPosts(int thread = -1)
        {
            var ctx = database.GetContext();

            return ctx.Threads.Where(x => x.ParentId == thread).OrderBy(x => x.Id).ToList();
        }

        [HttpPost("create")]
        public ActionResult<bool> CreatePost([FromBody] Thread newpost)
        {
            var ctx = database.GetContext();

            newpost.Id = 0;

            if (string.IsNullOrEmpty(newpost.Text))
                return BadRequest("text too short");

            if (string.IsNullOrEmpty(newpost.Username))
                newpost.Username = "Anonymous";

            ctx.Threads.Add(newpost);
            ctx.SaveChanges();
            return true;
        }
    }
}
