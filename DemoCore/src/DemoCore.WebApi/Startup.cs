﻿using System.IO;
using DemoCore.Infra.CrossCutting.Identity.Authorization;
using DemoCore.Infra.CrossCutting.Identity.Data;
using DemoCore.Infra.CrossCutting.Identity.Extensions;
using DemoCore.Infra.CrossCutting.Identity.Migrations;
using DemoCore.Infra.CrossCutting.Identity.Models;
using DemoCore.Infra.CrossCutting.IoC;
using DemoCore.Infra.Data.Context;
using DemoCore.Infra.Data.Migrations;
using DemoCore.WebAPI.Configurations;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace DemoCore.WebApi
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                //builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DemoCoreContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
            sqlServerOptions =>
            {
                sqlServerOptions.MigrationsAssembly("DemoCore.Infra.Data.Context");
            }));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
            sqlServerOptions =>
            {
                sqlServerOptions.MigrationsAssembly("DemoCore.Infra.CrossCutting.Identity");
            }));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddWebApi(options =>
            {
                options.OutputFormatters.Remove(new XmlDataContractSerializerOutputFormatter());
                options.UseCentralRoutePrefix(new RouteAttribute("api/v{version}"));
            });

            services.AddAutoMapperSetup();

            services.AddPoliciesSetup();

            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "DemoCore Project",
                    Description = "DemoCore API",
                    Contact = new Contact { Name = "Hugo Terra", Email = "hugo@sharpnet.com.br", Url = "http://www.sharpnet.com.br" }
                });
            });

            // Adding MediatR for Domain Events and Notifications
            services.AddMediatR(typeof(Startup));

            services.AddMvc(options =>
            {
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());   //Only in Presentation
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // .NET Native DI Abstraction
            RegisterServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IHttpContextAccessor accessor, DemoCoreContext context, ApplicationDbContext identityContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            app.UseCors(c =>
            {
                c.AllowAnyHeader();
                c.AllowAnyMethod();
                c.AllowAnyOrigin();
            });

            //app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "DemoCore Project API v1.0");
            });

            new DemoCoreDbInitializer(context).Seed().Wait();
            new IdentityDbInitializer(identityContext, userManager, roleManager).Seed();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // Adding dependencies from another layers (isolated from Presentation)
            NativeInjectorBootstrapper.RegisterServices(services);
        }
    }
}
