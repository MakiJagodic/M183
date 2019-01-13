using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using SingleSignOn.Models;

namespace SingleSignOn.App_Start
{
    public partial class Startup
    {
        // Weitere Informationen zum Konfigurieren der Authentifizierung finden Sie unter "http://go.microsoft.com/fwlink/?LinkId=301864".
        public void ConfigureAuth(IAppBuilder app)
        {
            // Konfigurieren des db-Kontexts, des Benutzer-Managers und des Anmelde-Managers für die Verwendung einer einzelnen Instanz pro Anforderung.
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Aktiviert die Anwendung für das vorübergehende Speichern von Benutzerinformationen beim Überprüfen der zweiten Stufe im zweistufigen Authentifizierungsvorgang.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
            
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // nicht komplett weitergekommen..
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "",
                ClientSecret = ""
            });
        }
    }
}