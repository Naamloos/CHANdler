using Chandler.Data;
using Chandler.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Chandler.Controllers
{
    /// <summary>
    /// Server object
    /// </summary>
    [ApiController, Route("api/[controller]"), Produces("application/json")]
    public class ServerController : ControllerBase
    {
        private readonly Database database;
        private readonly ServerMeta meta;

        /// <summary>
        /// Server Ctor
        /// </summary>
        /// <param name="database"></param>
        /// <param name="meta"></param>
        public ServerController(Database database, ServerMeta meta)
        {
            this.database = database;
            this.meta = meta;
        }

        /// <summary>
        /// Get server meta
        /// </summary>
        /// <returns>ServerMeta</returns>
        [HttpGet]
        public ActionResult<ServerMeta> GetServerMeta() => this.meta;

        /// <summary>
        /// Returns the status of the server
        /// </summary>
        /// <returns>ServerHealth Object</returns>
        [HttpGet("health")]
        public async Task<ServerHealth> GetHealthStatus()
        {
            var sw = new Stopwatch();
            sw.Start();
            var sh = new ServerHealth();
            this.Response.ContentType = "application/health+json";
            this.Response.Headers.Add("Cache-Control", "max-age=3600");

            var notes = new List<string>();
            using var ctx = this.database.GetContext();
            var db = await ctx.Database.CanConnectAsync();
            var meta = GetServerMeta().Value;
            sh.Uptime = meta.UpTime;
            sh.DatabaseOk = db;
            if (!db) sh.Output += "The database is unavailable and could not be connected to\n";

            if (notes.Count == 0) sh.OverallStatus = "Pass";
            else sh.OverallStatus = "Fail";

            sh.Latency = sw.ElapsedMilliseconds;
            return sh;
        }
    }
}