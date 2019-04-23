using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chandler.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chandler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        private Database database;
        private ServerMeta meta;

        public ServerController(Database database, ServerMeta meta)
        {
            this.database = database;
            this.meta = meta;
        }

        [HttpGet]
        public ActionResult<ServerMeta> GetServerMeta()
        {
            return meta;
        }
    }
}