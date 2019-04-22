using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Chandler.Data;

namespace Chandler
{
    public class Startup
    {
        private Database _db;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _db = new Database(DatabaseProvider.InMemory, null);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<Database>(_db);

            var ctx = _db.GetContext();
            ctx.Database.EnsureCreated();
            ctx.Posts.Add(new Data.Entities.Post() { Id = 1, Text = "InMemory Test Post.", ParentId = 0, Image = ""});
            ctx.SaveChanges();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
