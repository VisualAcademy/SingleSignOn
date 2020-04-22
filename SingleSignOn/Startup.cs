using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SingleSignOn.Areas.Identity.Data;
using SingleSignOn.Configurations;
using SingleSignOn.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SingleSignOn
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // IdentityServer4 ��� �� ".well-known/openid-configuration" ��� Ȯ��
            services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.GetIds())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(Config.GetTestUsers())
                .AddDeveloperSigningCredential();

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); 

            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization(); 

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages(); 
            });

            // PM> Update-Database required
            CreateBuiltInUsersAndRoles(serviceProvider).Wait();
        }

        private async Task CreateBuiltInUsersAndRoles(IServiceProvider serviceProvider)
        {
            //[0] DbContext ��ü ����
            var dbContext = serviceProvider.GetRequiredService<SingleSignOnDbContext>();
            dbContext.Database.EnsureCreated(); // �����ͺ��̽��� �����Ǿ� �ִ��� Ȯ��

            // �⺻ ���� ����� �� ������ �ϳ��� ������(��, ó�� �����ͺ��̽� �����̶��)
            if (!dbContext.Users.Any() && !dbContext.Roles.Any())
            {
                string domainName = "dul.me";
                //[1] Groups(Roles): 
                //[1][1] ('Administrators', '������ �׷�', 'Group', '���� ���α׷��� �� �����ϴ� ���� �׷� ����')
                //[1][2] ('Everyone', '��ü ����� �׷�', 'Group', '���� ���α׷��� ����ϴ� ��� ����� �׷� ����')
                //[1][3] ('Users', '�Ϲ� ����� �׷�', 'Group', '�Ϲ� ����� �׷� ����')
                //[1][4] ('Guests', '������ �׷�', 'Group', '�Խ�Ʈ ����� �׷� ����')
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roleNames = { "Administrators", "Everyone", "Users", "Guests" };
                IdentityResult identityResult;
                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        // ��Ʈ�� �׷� ���� 
                        identityResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                //[2] Users
                //[2][1] Administrator
                // ('Administrator', '������', 'User', '���� ���α׷��� �� �����ϴ� ����� ����')
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                ApplicationUser administrator = await userManager.FindByEmailAsync($"administrator@{domainName}");
                if (administrator == null)
                {
                    administrator = new ApplicationUser()
                    {
                        UserName = $"administrator@{domainName}",
                        Email = $"administrator@{domainName}",
                    };
                    await userManager.CreateAsync(administrator, "Pa$$w0rd");
                }
                await userManager.AddToRoleAsync(administrator, "Administrators");
                await userManager.AddToRoleAsync(administrator, "Users");

                //[2][2] Guest
                // ('Guest', '�Խ�Ʈ �����', 'User', '�Խ�Ʈ ����� ����')
                ApplicationUser guest = await userManager.FindByEmailAsync($"guest@{domainName}");

                if (guest == null)
                {
                    guest = new ApplicationUser()
                    {
                        UserName = "Guest",
                        Email = $"guest@{domainName}",
                    };
                    await userManager.CreateAsync(guest, "Pa$$w0rd");
                }
                await userManager.AddToRoleAsync(guest, "Guests");

                //[2][3] Anonymous
                // ('Anonymous', '�͸� �����', 'User', '�͸� ����� ����')
                ApplicationUser anonymous = await userManager.FindByEmailAsync($"anonymous@{domainName}");
                if (anonymous == null)
                {
                    anonymous = new ApplicationUser()
                    {
                        UserName = "Anonymous",
                        Email = $"anonymous@{domainName}",
                    };
                    await userManager.CreateAsync(anonymous, "Pa$$w0rd");
                }
                await userManager.AddToRoleAsync(anonymous, "Guests");
            }
        }
    }
}
