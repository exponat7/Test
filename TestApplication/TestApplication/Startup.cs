using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using TestApplication.Areas.Identity.Initialization;
using TestApplication.Areas.Identity.Managers;
using TestApplication.Areas.Identity.Models;
using TestApplication.Areas.Identity.Subsystems.Database;
using TestApplication.Subsystems.Database;
using TestApplication.Subsystems.Logging;
using TestApplication.Subsystems.RequestProcessing;
using TestApplication.Subsystems.Settings;
using TestApplication.Subsystems.SignalR;

namespace TestApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        public static ApplicationSettings Settings { get; private set; }

        internal static ILoggerFactory LoggerFactory { get; private set; }

        internal static RequestsProcessing Processing;


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                //Любой неразлогиненый руками пользователь всегда в системе(для учета состояний пользователя)
                options.ExpireTimeSpan = TimeSpan.FromDays(1000000);
            });
            //
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<IdentityContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("IdentityConnection")));
            //
            services.AddDbContext<ApplicationContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("ApplicationConnection")));

            services.AddDefaultIdentity<CustomIdentityUser>(options =>
            {
                //Опции пароля
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                //Опции пользователя
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
                //Отключаем верификацию мыла
                options.User.RequireUniqueEmail = false;
                //Отключаем локаут
                options.Lockout.AllowedForNewUsers = false;
                //options.Lockout.MaxFailedAccessAttempts = 5;
            })
                .AddUserManager<CustomUserManager<CustomIdentityUser>>()
                .AddSignInManager<CustomSignInManager<CustomIdentityUser>>()
                .AddRoles<IdentityRole>()
                //.AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddEntityFrameworkStores<IdentityContext>();

            services.AddSignalR();

            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                //Позволяет моментально разлогинивать пользователей через UpdateSecurityStampAsync(user);
                options.ValidationInterval = TimeSpan.Zero;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //Инициализируем логирование в папке log
            loggerFactory.AddFile(Path.Combine(env.WebRootPath, "log"));
            LoggerFactory = loggerFactory;
            //
            Settings = new ApplicationSettings(Configuration);
            //

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<IncomingHub>("/incoming");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller}/{action}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            IServiceScopeFactory scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<CustomUserManager<CustomIdentityUser>>();
                var signInManager = scope.ServiceProvider.GetRequiredService<CustomSignInManager<CustomIdentityUser>>();
                //Инициализация дефолтными значениями
                await InitialDefaults.InitDefaults(userManager, roleManager);
                //Разлогиниваем всех пользователей при старте приложения(иначе при логине с другой машины будет 2 куки)
                await LogoutAll(signInManager, userManager);
                //Получаем хаб для оповещения пользователя о изменении статуса
                var incomingHubContext = app.ApplicationServices.GetService<IHubContext<IncomingHub>>();
                Processing = new RequestsProcessing(userManager, LoggerFactory.CreateLogger<RequestsProcessing>(), incomingHubContext);
                //Стартуем процесс обработки запросов
                Processing.Start();
            }
        }

        private async Task LogoutAll(CustomSignInManager<CustomIdentityUser> sm, CustomUserManager<CustomIdentityUser> um)
        {
            var users = um.Users.ToList();
            foreach(var usr in users)
            {
                await um.UpdateSecurityStampAsync(usr);
                await sm.DeleteUserFromLoggedIn(usr);
            }
        }
    }
}
