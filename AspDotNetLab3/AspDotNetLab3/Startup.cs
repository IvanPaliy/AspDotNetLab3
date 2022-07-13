using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspDotNetLab3
{
    public class Startup
    {
        public IConfiguration AppConfiguration { get; set; }

        public Startup()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("conf.json");
            AppConfiguration = builder.Build();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            app.UseSession();
            app.UseRouting();
            app.UseLog(AppConfiguration["logFile"]);

            var routeBuilder = new RouteBuilder(app);

            //Task_2
            //routeBuilder.MapRoute("/{controller}/{action}/{id:int?}",
            //async context =>
            //{
            //    context.Response.ContentType = "text/html; charset=utf-8";
            //    await context.Response.WriteAsync($"Route <b>{context.Request.Path}</b> is avalible");
            //});
            //routeBuilder.MapRoute("/{lang}/{controller}/{action}/{id:int?}",
            //async context =>
            //{
            //    context.Response.ContentType = "text/html; charset=utf-8";
            //    await context.Response.WriteAsync($"Route <b>{context.Request.Path}</b> is avalible");
            //});
            //app.UseRouter(routeBuilder.Build());



            routeBuilder.MapRoute("/session/add/{key}/{value}",
            async context =>
            {
                var key = $"{context.Request.Path}".Split("/")[3];
                var value = $"{context.Request.Path}".Split("/")[4];
                context.Session.SetString(key, value);
                await context.Response.WriteAsync($"Key with value added to the session");
            });
            routeBuilder.MapRoute("/session/view/{key}",
            async context =>
            {
                var key = $"{context.Request.Path}".Split("/")[3];
                var value = context.Session.GetString(key);
                if (value != null)
                {
                    await context.Response.WriteAsync($"<b>{key}</b> (key) = {value}(value)");
                    return;
                }
                await context.Response.WriteAsync($"No value was found for this key in the session");
            });


            routeBuilder.MapRoute("/cookie/add/{key}/{value}",
            async context =>
            {
                var key = $"{context.Request.Path}".Split("/")[3];
                var value = $"{context.Request.Path}".Split("/")[4];
                context.Response.Cookies.Append(key, value);
                await context.Response.WriteAsync($"Key with value added to the cookies");
            }
            );


            routeBuilder.MapRoute("/cookie/view/{key}",
            async context =>
            {
                var key = $"{context.Request.Path}".Split("/")[3];
                if (context.Request.Cookies.ContainsKey(key))
                {
                    var value = context.Request.Cookies[key];
                    await context.Response.WriteAsync($"<b>{key}</b> (key) = {value} (value)");
                    return;
                }
                await context.Response.WriteAsync($"No value was found for this key");
            });
            app.UseRouter(routeBuilder.Build());


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                   await context.Response.WriteAsync($"{AppConfiguration["title"]}\nHey!");
                });
            });
        }
    }
}
