using Chandler.Controllers.Accounts.Models;
using Chandler.Misc;
using Domain.EF.Entities.Main;
using Domain.Misc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NToastNotify;
using System.Threading.Tasks;

namespace Chandler.Controllers
{
    /// <summary>
    /// Controller for pages relating to accounts
    /// </summary>
    [Route("[controller]"), ApiExplorerSettings(IgnoreApi = true), Authorize, AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private ServerConfig Config { get; }
        private SignInManager<ChandlerUser> SignInManager { get; }
        private UserManager<ChandlerUser> UserManager { get; }
        private IToastNotification ToastNotification { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="conf">Server Config</param>
        public AccountController(ServerConfig conf, SignInManager<ChandlerUser> signInManager, 
            UserManager<ChandlerUser> userManager, IToastNotification toastNotification)
        {
            this.Config = conf;
            this.SignInManager = signInManager;
            this.UserManager = userManager;
            this.ToastNotification = toastNotification;
        }

        /// <summary>
        /// Login using discord
        /// </summary>
        /// <returns>Discord Challenge</returns>
        [HttpGet("login/discord"), AllowAnonymous]
        public IActionResult LoginWithDiscord() => this.Challenge(new AuthenticationProperties() { RedirectUri = this.Config.DiscordOAuthSettings.RedirectUri }, "Discord");

        /// <summary>
        /// Returns login page
        /// </summary>
        /// <returns>Login Page View</returns>
        [HttpGet("login"), AllowAnonymous]
        public IActionResult Login() => this.View("/Views/Action/Login.cshtml", new LoginViewModel()
        {
            ServerConfig = this.Config
        });

        /// <summary>
        /// Login via form
        /// <returns></returns>
        [HttpPost("login"), AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = (await this.UserManager.FindByNameAsync(model.Username)) ?? await this.UserManager.FindByEmailAsync(model.Username);
            if (user == null) return this.Redirect("/account/login").WithError(this.ToastNotification, "Failed to Login");
            var result = await this.SignInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (result.Succeeded) return this.Redirect("/");
            else return this.Redirect("/account/login").WithError(this.ToastNotification, "Failed to Login");
        }

        /// <summary>
        /// Return register page
        /// </summary>
        /// <returns>Returns register page</returns>
        [HttpGet("register"), AllowAnonymous]
        public IActionResult Register() => this.View("/Views/Action/Register.cshtml", this.Config);

        ///// <summary>
        ///// Register via form
        ///// </summary>
        ///// <param name="Username"></param>
        ///// <param name="Email"></param>
        ///// <param name="Password"></param>
        ///// <returns></returns>
        //[Route("account/register"), HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        //public async Task<IActionResult> RegisterWithPost([FromForm]string Username, [FromForm]string Email, [FromForm]string Password)
        //{
        //}

        /// <summary>
        /// Logout
        /// </summary>
        /// <returns>Index page</returns>
        [HttpGet("logout"), Authorize]
        public async Task<IActionResult> Logout()
        {
            await this.SignInManager.SignOutAsync();
            return this.Redirect("/account/login");
        }

        ///// <summary>
        ///// Delete account page
        ///// </summary>
        ///// <returns>DeleteAccount Page</returns>
        //[Route("userdelete"), HttpGet, Authorize]
        //public IActionResult Delete() => this.View("/Views/Action/DeleteAccount.cshtml", this.Config);

        ///// <summary>
        ///// Deletes account
        ///// </summary>
        ///// <returns>IndexPage</returns>
        //[Route("account/delete"), HttpPost, Authorize, ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteFromPost([FromForm]string UsernameOrEmail, [FromForm]string Password)
        //{
        //}
    }
}
