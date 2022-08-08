using System.Configuration;
using System.Globalization;
using System.Threading.Tasks;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security.Notifications;
using Microsoft.IdentityModel.Protocols;
using System;

namespace LibraryManagement
{
    public partial class Startup
    {

        // Get key values from web.config file
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        private static string addInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];

        // Concatenate aadInstance, tenant to form authority value 
        private string authority = string.Format(CultureInfo.InvariantCulture + addInstance + tenant);

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            try
            {

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                //CookieManager = new SystemWebCookieManager()
                AuthenticationType = "Cookies",
                ExpireTimeSpan = TimeSpan.FromMinutes(60), // Expire after an hour
                SlidingExpiration = true // use sliding expiration..
            });
            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = authority,
                    PostLogoutRedirectUri = postLogoutRedirectUri,
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        AuthenticationFailed = (context) =>
                        {
                            context.HandleResponse();
                            context.OwinContext.Response.Redirect("/Home/Index");
                            return Task.FromResult(0);
                        }
                    },
                    UseTokenLifetime = false
                });
            }
            catch (TaskCanceledException ex)
            {
                    
            }
        }
    }
}
