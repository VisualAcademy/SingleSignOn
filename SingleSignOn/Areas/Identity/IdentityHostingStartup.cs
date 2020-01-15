using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SingleSignOn.Areas.Identity.Data;
using SingleSignOn.Data;

[assembly: HostingStartup(typeof(SingleSignOn.Areas.Identity.IdentityHostingStartup))]
namespace SingleSignOn.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<SingleSignOnDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("SingleSignOnDbContextConnection")));

                // From: 
                //services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                //    .AddEntityFrameworkStores<SingleSignOnDbContext>();
                // To:
                services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<SingleSignOnDbContext>().AddDefaultTokenProviders();
            });
        }
    }
}
