using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WCS.Data;
using WCS.Models;
using WCS.Services;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Mvc;

namespace WCS
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
            services.AddDbContext<WcsContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //UseSqlServer(Configuration.GetConnectionString("ProdConnection")));

            services.AddIdentity<User, IdentityRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = false; //TODO turn on the email verification
            })
                .AddEntityFrameworkStores<WcsContext>()
                .AddDefaultTokenProviders();

            // Configure Password and User Settings
            services.Configure<IdentityOptions>(options =>
            {
                // Password Options
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 3;

                options.User.RequireUniqueEmail = true;
                options.Lockout.AllowedForNewUsers = false;
            });

            // Configure Cookie Settings
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie Settings
                options.Cookie.HttpOnly = true;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });

            // Configure SSL Settings
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc().AddJsonOptions(
            options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, WcsContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Redirect to Https if request sent to Http
            RewriteOptions options = new RewriteOptions().AddRedirectToHttps();
            app.UseRewriter(options);

            /* Create Roles for the application */
           CreateRoles(serviceProvider);
        }

        private void CreateRoles(IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            UserManager<User> userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            Task<IdentityResult> roleResult;
            #region Administrator Role
            Task<bool> appHasAdminRole = roleManager.RoleExistsAsync("Administrator");
            appHasAdminRole.Wait();

            if (!appHasAdminRole.Result)
            {
                roleResult = roleManager.CreateAsync(new IdentityRole("Administrator"));
                roleResult.Wait();
            }

            #endregion
            #region Judge Role
            Task<bool> appHasJudgeRole = roleManager.RoleExistsAsync("Judge");
            appHasJudgeRole.Wait();

            if (!appHasJudgeRole.Result)
            {
                roleResult = roleManager.CreateAsync(new IdentityRole("Judge"));
                roleResult.Wait();
            }
            #endregion
            #region Student Role
            Task<bool> appHasStudentRole = roleManager.RoleExistsAsync("Student");
            appHasStudentRole.Wait();

            if (!appHasStudentRole.Result)
            {
                roleResult = roleManager.CreateAsync(new IdentityRole("Student"));
                roleResult.Wait();
            }
            #endregion
            #region Create Default Accounts
            #region Admin Account AdminPassword1
            string adminUserEmail = "wcs@weber.edu";
            Task<User> retrieveAdminUser = userManager.FindByEmailAsync(adminUserEmail);
            retrieveAdminUser.Wait();
            if (retrieveAdminUser.Result == null)
            {
                User administrator = new User
                {
                    Email = adminUserEmail,
                    UserName = adminUserEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true
                };

                Task<IdentityResult> addAdminUser = userManager.CreateAsync(administrator, "AdminPassword1");
                addAdminUser.Wait();
                if (addAdminUser.Result.Succeeded)
                {
                    Task<IdentityResult> addAdminRole = userManager.AddToRoleAsync(administrator, "Administrator");
                    addAdminRole.Wait();
                }
            }
            #endregion
            #region Judge Account JudgePassword1
            string judgeUserEmail = "wcsjudge@weber.edu";
            Task<User> retrieveJudgeUser = userManager.FindByEmailAsync(judgeUserEmail);
            retrieveJudgeUser.Wait();
            if (retrieveJudgeUser.Result == null)
            {
                User judge = new User
                {
                    Email = judgeUserEmail,
                    UserName = judgeUserEmail,
                    FirstName = "Judge",
                    LastName = "Dredd",
                    EmailConfirmed = true
                };

                Task<IdentityResult> addJudgeUser = userManager.CreateAsync(judge, "JudgePassword1");
                addJudgeUser.Wait();
                if (addJudgeUser.Result.Succeeded)
                {
                    Task<IdentityResult> addJudgeRole = userManager.AddToRoleAsync(judge, "Judge");
                    addJudgeRole.Wait();
                }
            }
            #endregion
            #endregion
        }
    }
}
