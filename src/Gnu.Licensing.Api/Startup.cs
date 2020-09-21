using System;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Gnu.Licensing.Svr.Infrastructure.Filters;
using Gnu.Licensing.Svr.Interface;
using Gnu.Licensing.Svr.Services;


namespace Gnu.Licensing.Svr
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
            .Services
            .AddHealthChecks(Configuration)
            .AddHttpClientServices(Configuration)
            .AddCustomMvc(Configuration);

            services.AddSwaggerGen();

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }

            app.UseForwardedHeaders();
            app.UsePathBase(options.PathBase);
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gnu.Licensing.Svr V1");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                }).RequireAuthorization(); // TODO: this require authorization which is not there??
            });
        }
    }

    static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddCheck<SignKeyHealthCheck>("license-sign-key-check", tags: new[] { "sign-key" })
                .AddSqlite(sqliteConnectionString: configuration.GetConnectionString("EfDbContext"))
                .AddUrlGroup(new Uri(configuration["SvrUrlHC"]), name: "shot-svr-check", tags: new string[] { "shot.svr" });

            return services;
        }

        public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<AppSettings>(configuration);

            return services;
        }

        public static IServiceCollection AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ILicenseService, LicenseService>();
           // services.AddDbContext<EfDbContext>(options => options.UseSqlite(configuration.GetConnectionString("EfDbContext")), ServiceLifetime.Transient);
            services.AddScoped<SignKeyHealthCheck>();
            services.AddTransient<ICertificateService, CertificateService>();

            return services;
        }

    }
}
