using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Speet.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Speet.Controllers
{
    [AllowAnonymous, Route("account")]
    public class AccountController : Controller
    {
        private readonly DatabaseContext _db;

        public AccountController(DatabaseContext db)
        {
            _db = db;
        }

        [Route("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [Route("Index")]
        public IActionResult GoogleResponse()
        {
            string googleId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (_db.User.Find(googleId) == null)
                CreateUser(googleId);

            Response.Cookies.Append(ApplicationConstants.GoogleIdCookieName, googleId, new CookieOptions()
            {
                Expires = DateTimeOffset.MaxValue,
                Path = "/",
                HttpOnly = true,
                Secure = true,
            });

            return RedirectToAction("DiscoverGroups", "SportGroup");
        }

        private void CreateUser(string userId)
        {
            User newUser = new User()
            {
                GoogleId = userId,
                Gender = GenderType.Male //Set gender from claim logic here
            };

            _db.User.Add(newUser);
            _db.SaveChanges();
        }

        [HttpGet, Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            Response.Cookies.Delete(ApplicationConstants.GoogleIdCookieName);
            return new RedirectResult(url: "/Site/Start", permanent: true, preserveMethod: true);
        }
    }
}
