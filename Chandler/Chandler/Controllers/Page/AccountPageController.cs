using Chandler.Data;
using Chandler.Data.Entities;
using Chandler.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chandler.Controllers
{
    /// <summary>
    /// Controller for pages relating to accounts
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AccountPageController : Controller
    {
        private const string INDEX_PAGE_PATH = "/Views/Main/Index.cshtml";
        private ServerConfig Config { get; }
        private Database Database { get; }
        private AccountController AccountController { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="conf">Server Config</param>
        /// <param name="db">Database</param>
        /// <param name="login">Login API Controller</param>
        public AccountPageController(ServerConfig conf, Database db, AccountController login)
        {
            this.Config = conf;
            this.Database = db;
            this.AccountController = login;
        }

        /// <summary>
        /// Returns login page
        /// </summary>
        /// <returns>Login Page View</returns>
        [Route("login"), HttpGet, AllowAnonymous]
        public IActionResult Login() => this.View("/Views/Action/Login.cshtml", new LoginPageModel() { Config = this.Config });

        /// <summary>
        /// Login via form
        /// </summary>
        /// <param name="UsernameOrEmail">Username or Email</param>
        /// <param name="Password">Password</param>
        /// <returns></returns>
        [Route("account/login"), HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithPost([FromForm]string UsernameOrEmail, [FromForm]string Password)
        {
            var res = await this.AccountController.LoginAsync(new AccountDetailsBody()
            {
                Email = UsernameOrEmail,
                Username = UsernameOrEmail,
                Password = Password
            });
            if (res.Result is BadRequestObjectResult badreq)
            {
                return this.View(INDEX_PAGE_PATH, new IndexPageModel()
                {
                    ActionStatus = new ApiActionStatus()
                    {
                        Message = $"Unsuccessful: {badreq.Value}",
                        ResponseCode = 400,
                        Title = "Login"
                    },
                    Boards = this.Database.Boards,
                    Config = this.Config
                });
            }
            else return this.LocalRedirect("/");
        }

        /// <summary>
        /// Return register page
        /// </summary>
        /// <returns>Returns register page</returns>
        [Route("register"), HttpGet, AllowAnonymous]
        public IActionResult Register() => this.View("/Views/Action/Register.cshtml", new LoginPageModel() { Config = this.Config });

        /// <summary>
        /// Register via form
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        [Route("account/register"), HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterWithPost([FromForm]string Username, [FromForm]string Email, [FromForm]string Password)
        {
            var details = new AccountDetailsBody()
            {
                Email = Email,
                Username = Username,
                Password = Password
            };
            var res = await this.AccountController.RegisterAccountAsync(details);
            if (res.Result is BadRequestObjectResult badreq)
            {
                return this.View(INDEX_PAGE_PATH, new IndexPageModel()
                {
                    ActionStatus = new ApiActionStatus()
                    {
                        Message = $"Unsuccessful: {badreq.Value}",
                        ResponseCode = 400,
                        Title = "Register"
                    },
                    Boards = this.Database.Boards,
                    Config = this.Config
                });
            }
            else
            {
                await this.AccountController.LoginAsync(details);
                return this.LocalRedirect("/");
            }
        }

        /// <summary>
        /// Logout
        /// </summary>
        /// <returns>Index page</returns>
        [Route("account/logout"), HttpGet, Authorize]
        public async Task<IActionResult> Logout()
        {
            await this.AccountController.LogoutAsync();
            return this.LocalRedirect("/");
        }
    }
}
