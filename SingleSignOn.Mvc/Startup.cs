using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SingleSignOn.Mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddAuthentication(options =>
            {
                // "Cookies" 문자열로 직접 설정해도 상관없음
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                // "OpenIdConnect"
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) // "Cookies"
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options => 
            {
                // IdentityServer4 주소 
                options.Authority = "https://localhost:44328/";

                // IdentityServer4에 지정한 ClientId 문자열과 동일한 값
                options.ClientId = "73b933f9-821e-47df-866d-ef97d24c7506";


                options.ResponseType = "code id_token";

                options.Scope.Add("openid");
                options.Scope.Add("profile");

                options.SaveTokens = true;

                options.ClientSecret = "73b933f9-821e-47df-866d-ef97d24c7506";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication(); // IdentityServer4를 위한 필수 설정
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
