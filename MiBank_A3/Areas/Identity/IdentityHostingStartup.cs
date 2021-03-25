using System;
using MiBank_A3.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(MiBank_A3.Areas.Identity.IdentityHostingStartup))]
namespace MiBank_A3.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<MiBank_A3IdentityDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("CustomerContext")));

                //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                //    .AddEntityFrameworkStores<MiBank_A3IdentityDbContext>();
            });
        }
    }
}