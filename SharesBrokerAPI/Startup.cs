using CurrencyConverterService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharesBrokerAPI.DatabaseAccess;
using SharesBrokerAPI.Domain.ExternalAPIs;
using SharesBrokerAPI.Domain.HTTP;
using SharesBrokerAPI.Options;

namespace SharesBrokerAPI
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
            services.AddDbContextPool<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("localTestConnection")));

            services.AddScoped<IShareRepository, SqlShareRepository>();
            services.AddScoped<IUserRepository, SqlUserRepository>();
            services.AddScoped<IUserShareRepository, SqlUserShareRepository>();
            services.AddScoped<IPurchaseRepository, SqlPurchaseRepository>();
            services.AddScoped<ISaleRepository, SqlSaleRepository>();
            services.AddScoped<IHTTPClientFactory, HttpClientFactory>();
            services.AddScoped<CurrencyConversionWSClient, CurrencyConversionWSClient>();
            services.AddScoped<ShareConverter, ShareConverter>();
            services.AddScoped<IRestShareAPIInvoker, AlphaVantageRestShareAPIInvoker>();

            services.AddControllers();

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Shares API", Version = "V1" });
            });

            services.AddCors();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            var swaggerOptions = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

            app.UseSwagger(option =>
            {
                option.RouteTemplate = swaggerOptions.JsonRoute;

            });

            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(swaggerOptions.UiEndpoint, swaggerOptions.Description);
            });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
