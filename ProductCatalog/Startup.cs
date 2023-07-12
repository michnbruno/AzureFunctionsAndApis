using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using ProductCatalog.Data;
using ProductCatalog.Models.DTOs;
using ProductCatalog.Services;
using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace ProductCatalog
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
            services.AddDbContext<ProductCatalogDbContext>(options =>
            options.UseSqlServer(
                Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ProductCatalogDbContext).Assembly.FullName)));


            services.AddScoped<IProductService, ProductService>();
            
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddHealthChecks();
           //     .AddDbContextCheck<ProductCatalogDbContext>("dbcontext", HealthStatus.Unhealthy);
            //services.AddSwaggerGen(options =>
            //{
            //    var contact = new OpenApiContact
            //    {
            //        Name = Configuration["SwaggerApiInfo:Name"],
            //    };

            //    options.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Title = $"{Configuration["SwaggerApiInfo:Title"]}",
            //        Version = "v1",
            //        Contact = contact
            //    });

            //    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //  //  options.IncludeXmlComments(xmlPath);
            //});
            services.Configure<GzipCompressionProviderOptions>(options =>
             options.Level = System.IO.Compression.CompressionLevel.Optimal);

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

        }

        public void Configure(IApplicationBuilder app)
        {

            app.UseSwagger();
            app.UseSwaggerUI();



            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();


            app
        .UseHealthChecks("/health/ping", new HealthCheckOptions { AllowCachingResponses = false })
        .UseHealthChecks("/health/dbcontext", new HealthCheckOptions { AllowCachingResponses = false });



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllers();
            });

            app.UseResponseCompression();

            app.UseExceptionHandler((appBuilder) =>
            {
                appBuilder.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    Exception exception = exceptionHandlerPathFeature?.Error;

                    context.Response.StatusCode = exception switch
                    {
                        EntityNotFoundException => StatusCodes.Status404NotFound,
                        _ => StatusCodes.Status500InternalServerError
                    };

                    ApiResponse apiResponse = exception switch
                    {
                        EntityNotFoundException => new ApiResponse("Product not found"),
                        _ => new ApiResponse("An error occurred")
                    };

                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(apiResponse));
                });
            });




            //app.UseSwagger();
            //app.UseSwaggerUI(options =>
            //{
            //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            //    options.RoutePrefix = string.Empty;
            //    options.DisplayRequestDuration();
            //});

            //app.UseRouting();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");

            //    endpoints.MapControllers();
            //});

        }
    }
}


public class BaseProductCatalogException : Exception
{ }

public class EntityNotFoundException : BaseProductCatalogException
{ }

namespace ProductCatalog.Models.DTOs
{
    public class ApiResponse
    {
        public ApiResponse(string message)
        {
            Message = message;
        }

        [JsonPropertyName("message")]
        public string Message { get; }
    }
}
