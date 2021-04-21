#pragma warning disable CS1591

using Domain.EF.Entities;
using Domain.EF.Entities.Main;
using Domain.Misc;
using Domain.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Chandler
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private ServerConfig ServerConfig { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var Currentdir = Directory.GetCurrentDirectory();
            var conffolderpath = Path.Combine(Currentdir, "config");
            var conffilepath = Path.Combine(Currentdir, "config", "config.json");

            //Make sure we have the folders we need even if -setup wasnt specified
            if (!Directory.Exists(conffolderpath)) Directory.CreateDirectory(conffolderpath);

            if (!File.Exists(conffilepath))
            {
                var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ServerConfig(), Formatting.Indented));
                var fs = File.Create(conffilepath);
                fs.WriteAsync(buffer, 0, buffer.Length).GetAwaiter().GetResult();
                fs.DisposeAsync().GetAwaiter().GetResult();
                Trace.WriteLine("A new configuration file was created. To customise, please either edit the newly created file or run the Setup Utility");
            }

            this.ServerConfig = JsonConvert.DeserializeObject<ServerConfig>(File.ReadAllText(conffilepath));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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

            services.AddCors(o => o.AddPolicy("publicpolicy", builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));

            services.AddRazorPages().AddRazorRuntimeCompilation();

            #region Auth and Forgery

            services.AddDbContext<ChandlerContext>(options =>
            {
                options.UseSqlite(this.ServerConfig.ConnectionString);
                options.UseLazyLoadingProxies();
            }, ServiceLifetime.Scoped);

            services.AddIdentity<ChandlerUser, IdentityRole<Guid>>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
                .AddEntityFrameworkStores<ChandlerContext>()
                .AddSignInManager<SignInManager<ChandlerUser>>()
                .AddUserManager<UserManager<ChandlerUser>>()
                .AddRoles<IdentityRole<Guid>>()
                .AddRoleManager<RoleManager<IdentityRole<Guid>>>()
                .AddTokenProvider<DataProtectorTokenProvider<ChandlerUser>>(TokenOptions.DefaultProvider);

            //if (this._config.DiscordOAuthSettings != null)
            //{
            //    var clientid = this._config.DiscordOAuthSettings.ClientId.ToString();
            //    var clientsecret = this._config.DiscordOAuthSettings.ClientSecret;
            //    using var crng = new RNGCryptoServiceProvider();
            //    var arr = new byte[15];
            //    crng.GetBytes(arr);
            //    var nonce = Convert.ToBase64String(arr);

            //    services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //        .AddDiscord(x =>
            //        {
            //            x.SignInScheme = "Identity.External";
            //            x.ClaimsIssuer = DiscordAuthenticationDefaults.Issuer;
            //            x.ReturnUrlParameter = "/";
            //            x.AccessDeniedPath = "/";
            //            x.ClientId = clientid;
            //            x.ClientSecret = clientsecret;
            //            x.TokenEndpoint = DiscordAuthenticationDefaults.TokenEndpoint;
            //            x.AuthorizationEndpoint = $"{DiscordAuthenticationDefaults.AuthorizationEndpoint}?response_type=code&client_id={clientid}&scope=identify&state={nonce}&redirect_uri={this._config.DiscordOAuthSettings.RedirectUri}";
            //            x.UserInformationEndpoint = DiscordAuthenticationDefaults.UserInformationEndpoint;
            //        });
            //}
            /*else */
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

            services.ConfigureApplicationCookie(x =>
            {
                x.LoginPath = "/account/login";
                x.LogoutPath = "/account/logout";
                x.AccessDeniedPath = "/account/login";
                x.ExpireTimeSpan = TimeSpan.FromDays(1);
                x.SlidingExpiration = true;
            });

            services.AddAntiforgery(x =>
            {
                x.HeaderName = "X-XSRF-TOKEN";
                x.FormFieldName = "X-XSRF-TOKEN";
                x.Cookie.HttpOnly = false;
                x.Cookie.IsEssential = true;
                x.SuppressXFrameOptionsHeader = false;
            });

            #endregion

            services.AddMvc(x => x.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddControllersAsServices()
                .AddNToastNotifyToastr();

            services.AddScoped<ThreadRepository>();
            services.AddScoped<BoardRepository>();
            services.AddSingleton(this.ServerConfig);
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development") app.UseDeveloperExceptionPage();

            //Just create the fucking tables you pile of donkey horse shit
            var ctx = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<ChandlerContext>();
            ctx.Database.EnsureCreated();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseNToastNotify();

            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                x.RoutePrefix = "docs";
            });

            app.UseCors("publicpolicy");
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Default",
                    template: "{controller=Home}/{Action=Index}");
            });
        }
    }
}