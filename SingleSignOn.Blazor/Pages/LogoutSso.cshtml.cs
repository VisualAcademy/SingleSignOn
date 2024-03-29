﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SingleSignOn.Blazor
{
    public class LogoutSsoModel : PageModel
    {
        public async Task OnGetAsync()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(
                OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
