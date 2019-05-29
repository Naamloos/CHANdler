using Chandler.Data;
using Microsoft.AspNetCore.Mvc;

namespace Chandler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        private readonly Database database;
        private readonly ServerMeta meta;

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