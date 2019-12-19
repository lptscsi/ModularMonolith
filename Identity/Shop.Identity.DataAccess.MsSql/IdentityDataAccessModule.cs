﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shop.Identity.Entities;
using Shop.Identity.Infrastructure.Interfaces.DataAccess;
using Shop.Utils.Modules;
using Shop.Utils.Connections;

namespace Shop.Identity.DataAccess.MsSql
{
    public class IdentityDataAccessModule : Module
    {
        public override void Load(IServiceCollection services)
        {
            services.AddDbContext<IIdentityDbContext, IdentityDbContext>((sp, bld) => 
            {
                var factory = sp.GetRequiredService<IConnectionFactory>();
                bld.UseSqlServer(factory.GetConnection());
            });
            
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<IdentityDbContext>();
        }
    }
}