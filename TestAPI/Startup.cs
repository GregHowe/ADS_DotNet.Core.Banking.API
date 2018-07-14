
namespace TestAPI
{  
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;  
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;  
    using Microsoft.EntityFrameworkCore;
    using Customer.Domain.Repository;
    using AutoMapper;
    using Automapper;
    using Common;
    using Common.Infrastructure.Repository;
    using Common.infrastructure.repository;
    using Customer.Application;
    using Transactions.application;
    using Transactions.Application;
    using BankAccount.Domain.Repository;
    using BankAccount.Application;
    using Security.application;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using System;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            #region Inject

            var MySqlConnection = Environment.GetEnvironmentVariable("BankingAPIConnectionString");
            Console.WriteLine(MySqlConnection);

            if (String.IsNullOrEmpty(MySqlConnection))
            {
                services.AddDbContext<BankingContext>(options => options.UseMySql(Configuration.GetConnectionString("MySqlConnection")));
            }
            else
            {
                services.AddDbContext<BankingContext>(options => options.UseMySql(MySqlConnection));
            }
            
            services.AddScoped<ICustomerApplicationService, CustomerApplicationService>();
            services.AddScoped<IBankAccountApplicationService, BankAccountApplicationService>();
            services.AddScoped<ITransactionApplicationService, TransactionApplicationService>();
            services.AddScoped<ISecurityApplicationService, SecurityApplicationService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddCors();

            #endregion

            #region MapperConfig

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutomapperProfile());
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            #endregion

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddTransient<DbInitializer>();

            var TokenSecret = Environment.GetEnvironmentVariable("BankingAPITokenSecret");
            Console.WriteLine(TokenSecret);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Jwt";  
                options.DefaultChallengeScheme = "Jwt";              
            }).AddJwtBearer("Jwt", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,                    
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenSecret)), 
                    ValidateLifetime = true, 
                    ClockSkew = TimeSpan.FromMinutes(5) 
                };
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, DbInitializer seeder)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseAuthentication();
            
            app.UseMvc();
        }
    }
}
