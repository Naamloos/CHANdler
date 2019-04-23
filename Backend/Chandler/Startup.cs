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
            services.AddCors(o => o.AddPolicy("publicpolicy", builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));

            var ctx = _db.GetContext();
            ctx.Database.EnsureCreated();

            ctx.Boards.Add(new Data.Entities.Board()
            {
                Name = "CHANdler",
                Tag = "c",
                Description = "CHANdler test board",
                ImageUrl = "https://i.kym-cdn.com/photos/images/newsfeed/000/779/388/d33.jpg"
            });

            ctx.Boards.Add(new Data.Entities.Board()
            {
                Name = "Random",
                Tag = "r",
                Description = "CHANdler test board / RANDOM",
            });

            ctx.SaveChanges();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("publicpolicy");
            app.UseMvc();
        }
    }
}
