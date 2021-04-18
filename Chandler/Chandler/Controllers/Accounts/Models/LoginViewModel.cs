﻿using Domain.Misc;

namespace Chandler.Controllers.Accounts.Models
{
    public class LoginViewModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public ServerConfig ServerConfig { get; set; }
    }
}
