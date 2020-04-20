using Chandler.Data;
using Chandler.Data.Entities;
using Chandler.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
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
        /// Login using discord
        /// </summary>
        /// <returns>Discord Challenge</returns>
        [Route("login/discord")]
        public IActionResult LoginWithDiscord() => this.Challenge(new AuthenticationProperties() { RedirectUri = this.Config.DiscordOAuthSettings.RedirectUri }, "Discord");

        /// <summary>
        /// Returns login page
        /// </summary>
        /// <returns>Login Page View</returns>
        [Route("login"), HttpGet, AllowAnonymous]
        public IActionResult Login() => this.View("/Views/Action/Login.cshtml", this.Config);

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
            else if (res.Result is ObjectResult obj && res.Result.GetType() != typeof(OkObjectResult))
            {
                this.HttpContext.Response.Headers.Add("X-Retry-After", ((ChandlerUser)obj.Value).LockoutEnd?.UtcDateTime.ToShortTimeString());

                return this.View(INDEX_PAGE_PATH, new IndexPageModel()
                {
                    ActionStatus = new ApiActionStatus()
                    {
                        Message = $"Unsuccessful: Too many login attempts. Try again later",
                        ResponseCode = 429,
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
        public IActionResult Register() => this.View("/Views/Action/Register.cshtml", this.Config);

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

        /// <summary>
        /// Delete account page
        /// </summary>
        /// <returns>DeleteAccount Page</returns>
        [Route("userdelete"), HttpGet, Authorize]
        public IActionResult Delete() => this.View("/Views/Action/DeleteAccount.cshtml", this.Config);

        /// <summary>
        /// Deletes account
        /// </summary>
        /// <param name="UsernameOrEmail">Username or Email of the user</param>
        /// <param name="Password">Password of the user</param>
        /// <returns>IndexPage</returns>
        [Route("account/delete"), HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFromPost([FromForm]string UsernameOrEmail, [FromForm]string Password)
        {
            var res = await this.AccountController.DeleteAccountAsync(this.User, new AccountDetailsBody()
            {
                Email = UsernameOrEmail,
                Username = UsernameOrEmail,
                Password = Password
            });

            if (res is BadRequestObjectResult badreq)
                return this.View(INDEX_PAGE_PATH, new IndexPageModel()
                {
                    ActionStatus = new ApiActionStatus()
                    {
                        Message = badreq.Value.ToString(),
                        ResponseCode = 500,
                        Title = "Delete Account"
                    },
                    Boards = this.Database.Boards,
                    Config = this.Config
                });

            else return this.LocalRedirect("/");
        }
    }
}
