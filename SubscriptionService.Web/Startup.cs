using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SubscriptionService.Web.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using SubscriptionService.Web.Exceptions;
using SubscriptionService.Web.Repositories;
using SubscriptionService.Web.Services;

namespace SubscriptionService.Web
{
    public class Startup
    {
        private ILogger<Startup> _logger;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddSwaggerGen(ConfigureSwaggerOptions);
            services.AddAutoMapper(typeof(Startup));
            services.AddTransient<IUserAccountRepository, UserAccountRepository>();
            services.AddTransient<IUserAccountService, UserAccountService>();

            services.Configure<DBConnectionStrings>(Configuration.GetSection(nameof(DBConnectionStrings)));
            services.AddTransient<IDBConnectionProvider, DBConnectionProvider>();
            services.AddMediatR(typeof(Startup));
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserService, UserService>();

            services.AddHealthChecks();

            services.AddControllers();
        }

        private void ConfigureSwaggerOptions(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1",
                new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "User - Account Api",
                    Description = "Service to manage Users and Accounts",
                    Version = "v1"
                });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            _logger = logger;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            //app.UseExceptionHandler(a => a.Run(HandleGlobalExceptions));
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });

            app.UseErrorHandling();
        }

        private async Task HandleGlobalExceptions(HttpContext context)
        {
            var exHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exHandlerPathFeature.Error;

            HttpStatusCode status = HttpStatusCode.InternalServerError;
            string message = "Error occured while processing the request";

            if (exception is ValidationException)
            {
                status = HttpStatusCode.BadRequest;
                message = exception.Message;
                if(((ValidationException)exception).Criticality != ValidationCriticality.Info)
                    _logger.LogWarning($"An error occured while processing the request: {exception.Message}");
            }
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            
        }
    }
}
