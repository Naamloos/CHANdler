using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chandler.Data;
using Chandler.Data.Entities;

namespace Chandler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private Database database;
        public PostController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Post>> Get()
        {
            var ctx = database.GetContext();

            var posts = new List<Post>();

            foreach(var p in ctx.Posts)
            {
                posts.Add(p);
            }

            return posts;
        }
    }
}
