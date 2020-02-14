using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameUserDB.Data;
using GameUserDB.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using Microsoft.Extensions.Primitives;

namespace GameUserDB
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string path = Directory.GetCurrentDirectory();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 443;
            });
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(1);
                options.ExcludedHosts.Add("www.example.com");//пример исключения 
            });

            //services.AddDbContext<ApplicationDbContext>(options =>
            //options.UseSqlServer(
            //    Configuration.GetConnectionString("ReserveConnection").Replace("[DataDirectory]",path)));
            services.AddDbContext<ApplicationDbContext>(options =>
                  options.UseNpgsql(
                    Configuration.GetConnectionString("PostgresHerokuConnection").Replace("[DataDirectory]", path)));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 4;
                options.User.RequireUniqueEmail = true;
            })
               .AddRoleManager<RoleManager<IdentityRole>>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultUI()
               .AddDefaultTokenProviders()
               .AddErrorDescriber<CustomIdentityErrorDescriber>();

            //
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<ApplicationDbContext>()
                .BuildServiceProvider();
            //

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("admin"));
                options.AddPolicy("RequireAdministratorOrUserRole", policy => policy.RequireRole("admin", "user"));
            });
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeFolder("/Account/Manage");//К этим страницам и папкам не имеют доступа неаутентифицированные пользователи
                options.Conventions.AuthorizeFolder("/UserManage", "RequireAdministratorRole");//только пользователи с ролью admin
                options.Conventions.AuthorizePage("/Account/Logout");
                options.Conventions.AuthorizePage("/NoScore");
                options.Conventions.AuthorizePage("/Index", "RequireAdministratorOrUserRole");
            });

            services.AddSingleton<IEmailSender, EmailSender>();

            services.ConfigureApplicationCookie(options =>//здесь можно определить пути для страниц
            {
                options.LoginPath = "/Account/Login";//так выглядит путь к странице авторизации

            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //app.UseForwardedHeaders();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.Use((context, next) =>
            {
                if (context.Request.Headers.TryGetValue("X-Forwarded-Proto", out StringValues proto))
                {
                    context.Request.Scheme = proto;
                }

                return next();
            });

            if (env.IsDevelopment())
            {
                
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
