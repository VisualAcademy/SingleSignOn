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
            // IdentityServer4 등록 후 ".well-known/openid-configuration" 경로 확인
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
            //[0] DbContext 개체 생성
            var dbContext = serviceProvider.GetRequiredService<SingleSignOnDbContext>();
            dbContext.Database.EnsureCreated(); // 데이터베이스가 생성되어 있는지 확인

            // 기본 내장 사용자 및 역할이 하나도 없으면(즉, 처음 데이터베이스 생성이라면)
            if (!dbContext.Users.Any() && !dbContext.Roles.Any())
            {
                string domainName = "dul.me";
                //[1] Groups(Roles): 
                //[1][1] ('Administrators', '관리자 그룹', 'Group', '응용 프로그램을 총 관리하는 관리 그룹 계정')
                //[1][2] ('Everyone', '전체 사용자 그룹', 'Group', '응용 프로그램을 사용하는 모든 사용자 그룹 계정')
                //[1][3] ('Users', '일반 사용자 그룹', 'Group', '일반 사용자 그룹 계정')
                //[1][4] ('Guests', '관리자 그룹', 'Group', '게스트 사용자 그룹 계정')
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roleNames = { "Administrators", "Everyone", "Users", "Guests" };
                IdentityResult identityResult;
                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        // 빌트인 그룹 생성 
                        identityResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                //[2] Users
                //[2][1] Administrator
                // ('Administrator', '관리자', 'User', '응용 프로그램을 총 관리하는 사용자 계정')
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
                // ('Guest', '게스트 사용자', 'User', '게스트 사용자 계정')
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
                // ('Anonymous', '익명 사용자', 'User', '익명 사용자 계정')
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
