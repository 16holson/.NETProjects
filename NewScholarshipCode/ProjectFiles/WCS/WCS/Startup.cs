using System;
using System.Threading.Tasks;
<<<<<<< HEAD
using Microsoft.AspNetCore.Builder;
=======
>>>>>>> tagedotnetupdates
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WCS.Data;
using WCS.Models;
using WCS.Services;
using Microsoft.AspNetCore.Rewrite;
<<<<<<< HEAD
using Microsoft.AspNetCore.Mvc;
=======
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
>>>>>>> tagedotnetupdates

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
<<<<<<< HEAD
=======


>>>>>>> tagedotnetupdates
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
<<<<<<< HEAD
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });
=======
>>>>>>> tagedotnetupdates

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

<<<<<<< HEAD
            services.AddMvc().AddJsonOptions(
            options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, WcsContext context)
=======
            services.Configure<CookiePolicyOptions>(options => {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

            services.AddControllersWithViews();
            services.AddRazorPages();

            //services.AddMvc().AddJsonOptions(
            //options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, WcsContext context)
>>>>>>> tagedotnetupdates
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
<<<<<<< HEAD
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
=======
                app.UseDatabaseErrorPage();

>>>>>>> tagedotnetupdates
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
<<<<<<< HEAD
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

=======
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}"); //MAY BREAK THINGS MAYBE
            });

            //app.UseMvc(routes => {
            //    routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
            //});

>>>>>>> tagedotnetupdates
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
