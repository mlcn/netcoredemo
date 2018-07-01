using GoogleSearchIntegration.Interfaces;
using GoogleSearchService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreDemo
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
            services.AddMvc();
            services.AddTransient<IGoogleSearchClient, SearchClient>();
            services.AddTransient<IGoogleSearchService, SearchService>();
            services.AddCors();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseCors(
                options => options.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
            );

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=MVCDemo}/{action=Index}");
            });
        }
    }
}
