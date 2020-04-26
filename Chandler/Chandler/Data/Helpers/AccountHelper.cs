using Chandler.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chandler.Data
{
    /// <summary>
    /// Helper class for accounts
    /// </summary>
    public class AccountHelper
    {
        private ServerConfig Config { get; set; }
        private SignInManager<ChandlerUser> SignInManager { get; set; }
        private UserManager<ChandlerUser> UserManager { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="usermanager">User Manager</param>
        /// <param name="signin">Sign in manager</param>
        /// <param name="conf">Server Config</param>
        public AccountHelper(UserManager<ChandlerUser> usermanager, SignInManager<ChandlerUser> signin, ServerConfig conf)
        {
            this.UserManager = usermanager;
            this.SignInManager = signin;
            this.Config = conf;
        }

        /// <summary>
        /// Find a user
        /// </summary>
        /// <param name="username">Username of the user to find</param>
        /// <param name="email">Email of the user to find</param>
        /// <returns></returns>
        public async Task<ChandlerUser> FindUserAsync(string username = "", string email = "")
        {
            var user = await this.UserManager.FindByNameAsync(username);
            if (user == null) user = await this.UserManager.FindByEmailAsync(email);
            return user;
        }

        /// <summary>
        /// Find a user
        /// </summary>
        /// <param name="claims">User claims</param>
        /// <returns>Chandler User</returns>
        public async Task<ChandlerUser> FindUserAsync(ClaimsPrincipal claims) => 
            await this.UserManager.GetUserAsync(claims).ConfigureAwait(false);

        /// <summary>
        /// Check db for admin users, if none, add a new one from config options
        /// </summary>
        /// <returns></returns>
        public async Task CheckForDefaultAdminAsync()
        {
            var newusr = new ChandlerUser()
            {
                UserName = this.Config.SiteConfig.AdminUsername,
                Email = this.Config.SiteConfig.AdminEmail,
                AdminInfo = new AdminInfo() { IsServerAdmin = true }
            };

            var res = await this.UserManager.CreateAsync(newusr, this.Config.SiteConfig.AdminPassword);

            if (!res.Succeeded) throw new Exception(res.Errors.First().Description);
        }

        /// <summary>
        /// Check cookie for current external login data
        /// </summary>
        /// <param name="ctx">Current HttpContext</param>
        /// <returns></returns>
        public async Task CheckExternalCookieAsync(HttpContext ctx)
        {
            var authres = await ctx.AuthenticateAsync("Identity.External");
            if (authres.Succeeded)
            {
                var props = authres.Ticket.Properties;
                var princ = authres.Principal;
                var claims = princ.Claims.ToList();
                var username = claims.First(x => x.Type == ClaimTypes.Name).Value;
                var id = ulong.Parse(claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var email = claims.First(x => x.Type == ClaimTypes.Email).Value;

                await this.SignInManager.SignInWithClaimsAsync(new ChandlerUser()
                {
                    UserName = username,
                    Email = email,
                    DiscordId = id
                }, props, claims);
            }
            else ctx.Response.Cookies.Delete("Identity.External");
        }
    }
}