﻿
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

            services.AddDbContext<BankingContext>(options => options.UseMySql(Configuration.GetConnectionString("MySqlConnection")));
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
            app.UseMvc();
        }
    }
}
