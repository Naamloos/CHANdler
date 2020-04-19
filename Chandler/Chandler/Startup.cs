#pragma warning disable CS1591

using AspNet.Security.OAuth.Discord;
using AspNetCoreRateLimit;
using Chandler.Data;
using Chandler.Data.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

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
            var Currentdir = Directory.GetCurrentDirectory();
            resfolderpath = Path.Combine(Currentdir, "res");
            var conffolderpath = Path.Combine(Currentdir, "config");
            var conffilepath = Path.Combine(Currentdir, "config", "config.json");

            //Make sure we have the folders we need even if -setup wasnt specified
            if (!Directory.Exists(conffolderpath)) Directory.CreateDirectory(conffolderpath);
            if (!Directory.Exists(resfolderpath)) Directory.CreateDirectory(resfolderpath);

            if (!File.Exists(conffilepath))
            {
                var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ServerConfig(), Formatting.Indented));
                var fs = File.Create(conffilepath);
                fs.WriteAsync(buffer, 0, buffer.Length).GetAwaiter().GetResult();
                fs.DisposeAsync().GetAwaiter().GetResult();
                Trace.WriteLine("A new configuration file was created. To customise, please either edit the newly created file or run the Setup Utility");
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
                this._db.Boards.Add(new Board()
                {
                    Name = "CHANdler",
                    Tag = "c",
                    Description = "CHANdler test board",
                    ImageUrl = "/res/logo.jpg"
                });

                this._db.Boards.Add(new Board()
                {
                    Name = "Random",
                    Tag = "r",
                    Description = "Random shit",
                });

                this._db.Boards.Add(new Board()
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

                (var hash, var salt) = Passworder.GenerateHash(this._config.SiteConfig.DefaultPassword, this._config.SiteConfig.DefaultPassword);

                this._db.Passwords.Add(new Password()
                {
                    Id = -1,
                    Hash = hash,
                    Salt = salt
                });

                this._db.SaveChanges();
            }

            #region Auth and Forgery

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

            if (this._config.DiscordOAuthSettings != null)
            {
                var clientid = this._config.DiscordOAuthSettings.ClientId.ToString();
                var clientsecret = this._config.DiscordOAuthSettings.ClientSecret;
                using var crng = new RNGCryptoServiceProvider();
                var arr = new byte[15];
                crng.GetBytes(arr);
                var nonce = Convert.ToBase64String(arr);

                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddDiscord(x =>
                    {
                        x.SignInScheme = "Identity.External";
                        x.ClaimsIssuer = DiscordAuthenticationDefaults.Issuer;
                        x.ReturnUrlParameter = "/";
                        x.AccessDeniedPath = "/";
                        x.ClientId = clientid;
                        x.ClientSecret = clientsecret;
                        x.TokenEndpoint = DiscordAuthenticationDefaults.TokenEndpoint;
                        x.AuthorizationEndpoint = $"{DiscordAuthenticationDefaults.AuthorizationEndpoint}?response_type=code&client_id={clientid}&scope=identify&state={nonce}&redirect_uri={this._config.DiscordOAuthSettings.RedirectUri}";
                        x.UserInformationEndpoint = DiscordAuthenticationDefaults.UserInformationEndpoint;
                    });
            }
            else services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

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

            #endregion

            services.AddMvc(x => x.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddControllersAsServices();

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Default",
                    template: "{controller=Page}/{Action=Index}");
            });
        }
    }
}