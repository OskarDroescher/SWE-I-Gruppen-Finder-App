using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Speet.Controllers
{
    [AllowAnonymous, Route("account")]
    public class AccountController : Controller
    {
        [Route("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);

            
        }

        [Route("Index")]
        public async Task<IActionResult> GoogleResponse()
        {
            /*
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Claims.ToList();
            var UserId = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            */

            /*
            var claims = result.Principal.Identities
                .FirstOrDefault().Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                });
            */

            //get NameIdentifier
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            // Cookie here
            Response.Cookies.Append(userId, "12345", new CookieOptions()
            {
                Expires = DateTimeOffset.Now.AddHours(4),
                Path = "/",
                HttpOnly = true,
                Secure = true,
            });

            return RedirectToAction("DiscoverGroups", "SportGroup");
        }
         

        [HttpGet, Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return new RedirectResult(url: "/SportGroup/DiscoverGroups", permanent: true, preserveMethod: true);


        }

        [HttpGet, Authorize]
        public IActionResult Login()
        {
            return View();
            
        }
    }
}
