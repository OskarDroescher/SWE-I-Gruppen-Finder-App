using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Speet.Models;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService;
using System.Linq;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Threading;
using Google.Apis.Util.Store;

namespace Speet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/account/google-login"; // Must be lowercase              
            })
            .AddGoogle(options =>
            {
                //IConfigurationSection googleAuthNSection = Configuration.GetSection("Authentication:Google");

                options.ClientId = Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];

                options.Scope.Add("https://www.googleapis.com/auth/user.gender.read");
                options.Scope.Add("https://www.googleapis.com/auth/user.birthday.read");
                options.Scope.Add("profile");

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "UserId");
                options.ClaimActions.MapJsonKey(ClaimTypes.Email, "EmailAddress", ClaimValueTypes.Email);
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "Name");
                options.ClaimActions.MapJsonKey(ClaimTypes.Gender, "Gender");
                options.ClaimActions.MapJsonKey(ClaimTypes.DateOfBirth, "Birthday");
                options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");

                options.SaveTokens = true;

            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddDbContext<DatabaseContext>();
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
                app.UseExceptionHandler("/Home/Error");
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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=SportGroup}/{action=DiscoverGroups}/{pageIndex?}");
            });
        }
    }
}
