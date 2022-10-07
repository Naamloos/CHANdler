using Microsoft.AspNetCore.Mvc;

namespace API
{
    /// <summary>
    /// Provides endpoints for controlling boards
    /// </summary>
    [ApiController, Route("api/v{version:apiVersion}/[controller]"), ApiVersion("1.0")]
    public class BoardsController : Controller
    {
        /// <summary>
        /// Get a list of currently active boards
        /// </summary>
        /// <returns>A simple list of boards</returns>
        [HttpGet]
        public async Task<string> GetBoardsListAsync() => "Test";
    }
}
