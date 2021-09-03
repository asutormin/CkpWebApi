using System.Text;
using CkpWebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using CkpDAL;
using CkpServices.Interfaces;
using CkpServices;
using Microsoft.AspNetCore.Http;
using CkpInfrastructure.Configuration;
using CkpWebApi.Middleware;

namespace CkpWebApi
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
            services.AddDbContext<BPFinanceContext>(opt =>
            {
                var appSettings = new AppSettings();
                Configuration.GetSection("AppSettings").Bind(appSettings);
                var dbName = appSettings.DatabaseName;
                var connectionString = string.Format(Configuration.GetConnectionString("BPFinance"), dbName);

                opt.UseSqlServer(connectionString);
            });

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            var appParamsSection = Configuration.GetSection("AppParams");
            services.Configure<AppParams>(appParamsSection);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IOrderPositionService, OrderPositionService>();
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<IPaymentService, PaymentService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionMiddleware>();
            // app.UseMiddleware<JwtMiddleware>();

            app.UseRouting();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
