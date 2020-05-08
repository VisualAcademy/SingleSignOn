using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SingleSignOn.Blazor.Data;

namespace SingleSignOn.Blazor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // "Cookies"
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; // "oidc"
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) // "Cookies"
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options => // "oidc"
            {
                // 다음 Authority 속성은 SSO 프로젝트의 로컬 또는 실제 URL 지정
                //options.Authority = "https://localhost:44328/"; // Identity Server URI
                options.Authority = "https://localhost:5001/"; // Identity Server URI
                options.ClientId = "6a297776-c6ae-49c6-8cae-e6ef10a92cf0"; // "BlazorClient" 

                options.ResponseType = "code id_token";
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.SaveTokens = true;
                options.ClientSecret = "6a297776-c6ae-49c6-8cae-e6ef10a92cf0"; // "secret"
                options.GetClaimsFromUserInfoEndpoint = true;
            });

            services.AddSingleton<WeatherForecastService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // 인증 정보 얻기
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
