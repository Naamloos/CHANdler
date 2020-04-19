using Chandler.Data;
using Chandler.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chandler.Controllers
{
    /// <summary>
    /// Login api controller
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AccountController : Controller
    {
        private ServerConfig Config { get; set; }
        private Database Database { get; set; }
        private AccountHelper Helper { get; set; }
        private UserManager<ChandlerUser> UserManager { get; set; }

        /// <summary>
        /// SignInManager for Accounts
        /// </summary>
        public SignInManager<ChandlerUser> SignInManager { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="conf">Server Config</param>
        /// <param name="db">Database</param>
        /// <param name="usermanager">User Manager</param>
        /// <param name="signmanager">Sign In Manager</param>
        public AccountController(ServerConfig conf, Database db, UserManager<ChandlerUser> usermanager, SignInManager<ChandlerUser> signmanager)
        {
            this.Config = conf;
            this.Database = db;
            this.Helper = new AccountHelper(usermanager);
            this.UserManager = usermanager;
            this.SignInManager = signmanager;
        }

        /// <summary>
        /// Log a user in
        /// </summary>
        /// <param name="details">Login details of the user. Username/Email and Password</param>
        /// <returns>ChandlerUser object on success</returns>
        /// <response code="400">If either the username or email are invalid</response>
        /// <response code="429">If too many logins were attempted</response>
        [AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<ActionResult<ChandlerUser>> LoginAsync([FromBody]AccountDetailsBody details)
        {
            if (this.Database.Users.Count(x => x.AdminInfo.IsServerAdmin) == 0)
            {
                var res = await this.UserManager.CreateAsync(new ChandlerUser()
                {
                    UserName = this.Config.SiteConfig.AdminUsername,
                    Email = this.Config.SiteConfig.AdminEmail,
                    AdminInfo = new AdminInfo() { IsServerAdmin = true }
                }, this.Config.SiteConfig.AdminPassword);

                if (!res.Succeeded) throw new Exception(res.Errors.First().Description);
            }

            if (details.Username == null && details.Email == null) return this.BadRequest("No username or email has been provided");
            var user = await this.Helper.FindUserAsync(details.Username, details.Email);
            if (user == null) return this.BadRequest("Username/Email or Password was incorrect");

            if (user.LockoutEnd > DateTime.Now)
            {
                this.HttpContext.Response.Headers.Add("X-Retry-After", user.LockoutEnd?.UtcTicks.ToString());
                return this.StatusCode(429, "Too many failed login requests");
            }

            var idenres = await this.SignInManager.PasswordSignInAsync(user, details.Password, true, true);
            if (idenres.Succeeded) return this.Ok(user);
            else
            {
                await this.UserManager.AccessFailedAsync(user);
                return this.BadRequest("Username/Email or Password was incorrect");
            }
        }

        /// <summary>
        /// Log the current user out
        /// </summary>
        /// <returns>Empty Response</returns>
        [Authorize]
        public async Task<ActionResult> LogoutAsync()
        {
            await this.SignInManager.SignOutAsync();
            return this.Ok();
        }

        /// <summary>
        /// Registers a new account
        /// </summary>
        /// <param name="details">Post body details of the new user. Username, email, and password.</param>
        /// <returns>ChandlerUser object on success</returns>
        /// <response code="400">If any of the query parameters are null or if the email has already been registered</response>
        /// <response code="500">If there is an error when creating the new user account</response>
        [AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<ActionResult<ChandlerUser>> RegisterAccountAsync([FromBody]AccountDetailsBody details)
        {
            if (string.IsNullOrEmpty(details.Email) || string.IsNullOrEmpty(details.Username) || string.IsNullOrEmpty(details.Password)) return this.BadRequest("Parameters cannot be null or empty");
            var user = await this.Helper.FindUserAsync(details.Username, details.Email);
            if (user != null) return this.BadRequest("Email is already registered");
            var newusr = new ChandlerUser()
            {
                UserName = details.Username,
                Email = details.Email
            };
            var idenres = await this.UserManager.CreateAsync(newusr, details.Password);
            if (idenres.Succeeded) return Ok(newusr);
            else return this.StatusCode(500, $"Unable to register new user: {idenres.Errors.First().Description}");
        }

        /// <summary>
        /// Deletes an account
        /// </summary>
        /// <param name="usrclaim">Current user's claims</param>
        /// <param name="details">Account details</param>
        /// <returns>200 OK on deletion</returns>
        [Authorize, ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAccountAsync(ClaimsPrincipal usrclaim, [FromBody]AccountDetailsBody details)
        {
            ChandlerUser user;
            if (usrclaim.Identity != null) user = await this.UserManager.GetUserAsync(usrclaim);
            else user = await this.Helper.FindUserAsync(details.Username, details.Email);
            if (user == null) return this.BadRequest("User does not exist");
            var threads = this.Database.Threads.Where(x => x.UserId == user.Id);
            this.Database.Threads.RemoveRange(threads);
            await this.SignInManager.SignOutAsync();
            var res = await this.UserManager.DeleteAsync(user);
            if (res.Succeeded)
            {
                await this.Database.SaveChangesAsync();
                return this.Ok("Account Deleted");
            }
            else return this.StatusCode(500, res.Errors.First().Description);
        }
    }
}
