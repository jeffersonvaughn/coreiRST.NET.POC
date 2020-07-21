using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace coreiWS
{
    public class Startup
    {


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddMvc().AddControllersAsServices();
            
            services.AddMvc()
                 .AddJsonOptions(options =>
                 {
                     options.JsonSerializerOptions.WriteIndented = true;
                 });

            services.AddHttpClient();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
            
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                   name: "endPointExecutionTimeOnly",
                  pattern: "{controller=Home}/{action=endPointExecutionTimeOnly}/{id?}");

                endpoints.MapControllerRoute(
                    name: "getTableLayout",
                    pattern: "{controller=Home}/{action=getTableLayout}/{id?}");

                endpoints.MapControllerRoute(
                     name: "getCustomerBankAccountInfo",
                     pattern: "{controller=Home}/{action=getCustomerBankAccountInfo}/{id?}");

                endpoints.MapControllerRoute(
                     name: "coreiOverview",
                     pattern: "{controller=Home}/{action=coreiOverview}/{id?}");

                endpoints.MapControllerRoute(
                     name: "modifyAPIRequest",
                     pattern: "{controller=Home}/{action=modifyAPIRequest}/{id?}");

                endpoints.MapControllerRoute(
                     name: "getListOfCustomers",
                     pattern: "{controller=Home}/{action=getListOfCustomers}/{id?}");


            });
        }
    }

}
