﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Chandler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
            .UseUrls("http://0.0.0.0:420");
    }
}
