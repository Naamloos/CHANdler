using Chandler.Models;
using Domain.Misc;
using Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chandler.Controllers.Home
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private BoardRepository BoardRepository { get; }
        private ServerConfig ServerConfig { get; }

        public HomeController(BoardRepository boardRepository, ServerConfig serverConfig)
        {
            this.BoardRepository = boardRepository;
            this.ServerConfig = serverConfig;
        }

        [HttpGet]
        public IActionResult Index() => this.View("/Views/Main/Index.cshtml", new IndexPageModel()
        {
            Boards = this.BoardRepository.GetAll(),
            Config = this.ServerConfig
        });
    }
}
