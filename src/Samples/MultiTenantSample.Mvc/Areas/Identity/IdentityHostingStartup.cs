using System;
using Eleven41.AspNetCore.MultiTenantIdentity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiTenantSample.Mvc.Data;

[assembly: HostingStartup(typeof(MultiTenantSample.Mvc.Areas.Identity.IdentityHostingStartup))]
namespace MultiTenantSample.Mvc.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}