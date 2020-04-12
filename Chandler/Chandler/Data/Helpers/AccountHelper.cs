using Chandler.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Chandler.Data
{
    /// <summary>
    /// Helper class for accounts
    /// </summary>
    public class AccountHelper
    {
        private UserManager<ChandlerUser> UserManager { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="usermanager">User Manager</param>
        public AccountHelper(UserManager<ChandlerUser> usermanager) => this.UserManager = usermanager;

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
    }
}
