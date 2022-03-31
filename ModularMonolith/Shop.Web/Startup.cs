﻿using System.Linq;
using Autofac;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shop.Order.DataAccess.MsSql;
using Shop.Order.UseCases;
using Shop.Web.Utils;
using Microsoft.Extensions.Hosting;
using Shop.Communication.BackgroundJobs;
using Shop.Communication.Contract.Implementation;
using Shop.Communication.DataAccess.MsSql;
using Shop.Order.Contract.Implementation;
using Shop.Communication.UseCases;
using Shop.Emails.Implementation;
using Shop.Emails.Interfaces;
using Shop.Framework.UseCases.Implementation;

namespace Shop.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public ILifetimeScope AutofacContainer { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddOptions();

            services.AddControllers()
                .ConfigureApplicationPartManager(manager => manager.FeatureProviders.Add(new InternalControllerFeatureProvider()));

            services.Configure<EmailOptions>(Configuration.GetSection("EmailOptions"));

            var sp = services.BuildServiceProvider();

            var requests = sp.GetServices<IBaseRequest>();
            //MediatR works when AddMediatR calls in each module
            services.AddMediatR(requests.Select(x => x.GetType()).ToArray());

            var profiles = sp.GetServices<Profile>();
            //AutoMapper not works when AddAutoMapper calls in each module
            services.AddAutoMapper(profiles.Select(x => x.GetType()).ToArray());
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(DbTransactionPipelineBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            
            builder.RegisterModule<CommunicationDataAccessModule>();
            builder.RegisterModule<OrderDataAccessModule>();

            builder.RegisterModule<FrameworkModule>();
            builder.RegisterModule<EmailModule>();

            builder.RegisterModule<CommunicationInfrastructureModule>();
            builder.RegisterModule<CommunicationContractModule>();
            builder.RegisterModule<CommunicationUseCasesModule>();

            builder.RegisterModule<OrderContractModule>();
            builder.RegisterModule<OrderUseCasesModule>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandlerMiddleware();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
