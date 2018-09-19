using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using MmWizard.Db;
using MmWizard.Helper;
using MmWizard.Models;
using MySql.Data.MySqlClient;
using StackExchange.Redis.Extender;

namespace MmWizard
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            // services.AddScoped<IViewRenderService, ViewRenderService>();
            var logger = services.AddLogging()
                .BuildServiceProvider()
                .GetService<ILoggerFactory>()
                .AddConsole();
            try
            {
                SiteConfig.SetAppSetting(Configuration.GetSection("Config"));
            }
            catch(Exception ex)
            {
                logger.CreateLogger("Cfg").LogError(ex, "配置失败,关闭系统");
                throw ex;
            }
            RedisConnectionManager.Logger = logger.CreateLogger("Redis");

            
            services.AddDbService(new DbConnOption(SiteConfig.GetConnString()));

            //检查redis和db
            //using (var conn = new MySqlConnection(SiteConfig.ConnString()))
            //{
            //    var db = RedisConnectionManager.GetDatabase(SiteConfig.RedisConfig(),2);
            //    db.Set("abc", "123");
            //}
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
