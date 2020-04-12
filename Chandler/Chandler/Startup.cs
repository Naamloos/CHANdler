#pragma warning disable CS1591

using AspNetCoreRateLimit;
using Chandler.Data;
using Chandler.Data.Entities;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Chandler
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private readonly ServerConfig _config;
        private readonly Database _db;
        private readonly ServerMeta _meta;
        private readonly string resfolderpath;

        public Startup(IConfiguration configuration)
        {

            Configuration = configuration;

            var currentdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(currentdir);

            resfolderpath = Path.Combine(currentdir, "res");
            var conffolderpath = Path.Combine(currentdir, "config");
            var conffilepath = Path.Combine(currentdir, "config/config.json");

            // Ensure directories exist
            if (!Directory.Exists(resfolderpath))
                Directory.CreateDirectory(resfolderpath);

            if (!Directory.Exists(conffolderpath))
                Directory.CreateDirectory(conffolderpath);

            if (!File.Exists(conffilepath))
            {
                File.Create(conffilepath).Close();
                File.WriteAllText(conffilepath, JsonConvert.SerializeObject(new ServerConfig(), Formatting.Indented));
            }

            _config = JsonConvert.DeserializeObject<ServerConfig>(File.ReadAllText(conffilepath));
            _db = new Database(_config.Provider, _config.ConnectionString);
            _meta = new ServerMeta();
        }

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

            services.AddSingleton(_db);
            services.AddSingleton(_meta);
            services.AddSingleton(_config);
            services.AddCors(o => o.AddPolicy("publicpolicy", builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));

            this._db.Database.EnsureCreated();

            if (this._db.Boards.Count() == 0)
            {
                // insert debug thread data to database
                this._db.Boards.Add(new Data.Entities.Board()
                {
                    Name = "CHANdler",
                    Tag = "c",
                    Description = "CHANdler test board",
                    ImageUrl = "/res/logo.jpg"
                });

                this._db.Boards.Add(new Data.Entities.Board()
                {
                    Name = "Random",
                    Tag = "r",
                    Description = "Random shit",
                });

                this._db.Boards.Add(new Data.Entities.Board()
                {
                    Name = "Memes",
                    Tag = "m",
                    ImageUrl = "/res/pepo.gif",
                    Description = "haha cool and good dank memes",
                });

                this._db.Boards.Add(new Board()
                {
                    Name = "Meta",
                    Tag = "meta",
                    ImageUrl = "/res/wrench.png",
                    Description = "About CHANdler itself, e.g. development talk.",
                });

                (var hash, var salt) = Passworder.GenerateHash(this._config.DefaultPassword, this._config.DefaultPassword);

                this._db.Passwords.Add(new Password()
                {
                    Id = -1,
                    Hash = hash,
                    Salt = salt
                });

                this._db.SaveChanges();
            }

            services.AddIdentity<ChandlerUser, IdentityRole>(x =>
            {
                x.Password.RequiredLength = 8;
                x.Password.RequiredUniqueChars = 0;
                x.Password.RequireDigit = false;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;

                x.User.RequireUniqueEmail = true;

                x.Lockout.AllowedForNewUsers = true;
                x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                x.Lockout.MaxFailedAccessAttempts = 5;

                x.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@_-+=?/!\\ ";
            }).AddEntityFrameworkStores<Database>()
            .AddDefaultTokenProviders()
            .AddUserManager<UserManager<ChandlerUser>>()
            .AddSignInManager<SignInManager<ChandlerUser>>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

            services.ConfigureApplicationCookie(x =>
            {
                x.LoginPath = "/login";
                x.LogoutPath = "/logout";
                x.AccessDeniedPath = "/";
                x.ExpireTimeSpan = TimeSpan.FromDays(1);
                x.SlidingExpiration = true;
            });

            services.AddAntiforgery(x =>
            {
                x.FormFieldName = "AntiForgeryToken";
                x.HeaderName = "X-CRSF-TOKEN";
            });

            services.AddMvc(x => x.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddControllersAsServices();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAntiforgery antiforgery)
        {
            if (env.EnvironmentName == "Development") app.UseDeveloperExceptionPage();

            app.UseAuthentication();
            app.UseIpRateLimiting();

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
                FileProvider = new PhysicalFileProvider(resfolderpath),
                RequestPath = "/res"
            });

            //app.Use(req => ctx =>
            //{
            //    var tokens = antiforgery.GetAndStoreTokens(ctx);
            //    ctx.Response.Cookies.Append("CRSF-TOKEN", tokens.RequestToken, new CookieOptions()
            //    {
            //        HttpOnly = false
            //    });
            //    return req(ctx);
            //});
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