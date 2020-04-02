#pragma warning disable CS1591

using AspNetCoreRateLimit;
using Chandler.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using Chandler.Data.Entities;
using Microsoft.Extensions.FileProviders;

namespace Chandler
{
    public class Startup
    {
        private readonly ServerConfig _config;
        private readonly Database _db;
        private readonly ServerMeta _meta;
        private string currentdir;

        public Startup(IConfiguration configuration)
        {

            Configuration = configuration;

            currentdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(currentdir);
            // Ensure directories exist
           if (!Directory.Exists(Path.Combine(currentdir, "res")))
                Directory.CreateDirectory(Path.Combine(currentdir, "res"));

            if (!Directory.Exists(Path.Combine(currentdir, "config")))
                Directory.CreateDirectory(Path.Combine(currentdir, "config"));

            if (!File.Exists(Path.Combine(currentdir, "config/config.json"))) 
            {
                File.Create(Path.Combine(currentdir, "config/config.json")).Close();
                File.WriteAllText(Path.Combine(currentdir, "config/config.json"), 
                    JsonConvert.SerializeObject(new ServerConfig()));
            }

            _config = JsonConvert.DeserializeObject<ServerConfig>(
                File.ReadAllText(Path.Combine(currentdir, "config/config.json")));
            
            _db = new Database(_config.Provider, _config.ConnectionString);
            _meta = new ServerMeta();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region AspNetCoreRateLimit Stuff
            //Taken from https://github.com/stefanprodan/AspNetCoreRateLimit/wiki/IpRateLimitMiddleware#setup
            services.AddOptions();
            services.AddMemoryCache();

            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            #endregion

            #region General Chandler Stuff
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "CHANdler API Documentation",
                    Version = "v1",
                    License = new OpenApiLicense()
                    {
                        Name = "GNU General Public License v3.0",
                        Url = new Uri("https://github.com/Naamloos/CHANdler/blob/master/LICENSE")
                    },
                });

                x.IncludeXmlComments($"{AppContext.BaseDirectory}/{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            });

            services.AddMvc(x => x.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddControllersAsServices();
            services.AddSingleton(_db);
            services.AddSingleton(_meta);
            services.AddSingleton(_config);
            services.AddCors(o => o.AddPolicy("publicpolicy", builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));

            using var ctx = _db.GetContext();
            ctx.Database.EnsureCreated();

            if (ctx.Boards.Count() == 0)
            {
                // insert debug thread data to database
                ctx.Boards.Add(new Data.Entities.Board()
                {
                    Name = "CHANdler",
                    Tag = "c",
                    Description = "CHANdler test board",
                    ImageUrl = "/res/logo.jpg"
                });

                ctx.Boards.Add(new Data.Entities.Board()
                {
                    Name = "Random",
                    Tag = "r",
                    Description = "Random shit",
                });

                ctx.Boards.Add(new Data.Entities.Board()
                {
                    Name = "Memes",
                    Tag = "m",
                    ImageUrl = "/res/pepo.gif",
                    Description = "haha cool and good dank memes",
                });

                ctx.Boards.Add(new Board()
                {
                    Name = "Meta",
                    Tag = "meta",
                    ImageUrl = "/res/wrench.png",
                    Description = "About CHANdler itself, e.g. development talk.",
                });

                var hash = Passworder.GenerateHash(this._config.DefaultPassword, this._config.DefaultPassword);

                ctx.Passwords.Add(new Password()
                {
                    Id = -1,
                    Salt = hash.Salt,
                    Hash = hash.Hash
                });

                ctx.SaveChanges();
            }
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development") app.UseDeveloperExceptionPage();

            //app.UseIpRateLimiting();

            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                x.RoutePrefix = "docs";
            });

            app.UseCors("publicpolicy");
            app.UseStaticFiles();

            app.UseFileServer(new FileServerOptions() 
            { 
                FileProvider = new PhysicalFileProvider(Path.Combine(currentdir, "res")),
                RequestPath = "/res"
            });

            //app.UseHttpsRedirection();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Default",
                    template: "{controller=Page}/{Action=Index}");
            });
        }
    }
}