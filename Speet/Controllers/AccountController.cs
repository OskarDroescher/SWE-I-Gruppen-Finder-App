using Google.Apis.Auth.OAuth2;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Speet.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Speet.Controllers
{
    [AllowAnonymous, Route("account")]
    public class AccountController : Controller
    {
        private readonly DatabaseContext _db;
        public IConfiguration Configuration { get; }

        static string[] Scopes = {
                "profile",
                "https://www.googleapis.com/auth/user.gender.read",
                "https://www.googleapis.com/auth/user.birthday.read"
            };

        public AccountController(DatabaseContext db, IConfiguration configuration)
        {
            _db = db;
            Configuration = configuration;      
        }

        [Route("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { IsPersistent = true, RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public IActionResult GoogleResponse()
        {
            string googleId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            User user = _db.User.Find(googleId);

            if (user == null)
                user = CreateUser(googleId);

            if (user.PictureUrl != User.FindFirst("urn:google:picture")?.Value)
                UpdateProfilePicture(user);

            return RedirectToAction("DiscoverGroups", "SportGroup");
        }

        private User CreateUser(string userId)
        {
            Person UserInfo = GetGoogleProfile();

            User newUser = new User()
            {
                GoogleId = userId,
                Username = User.Identity.Name,
                Birthday = GetBirthday(UserInfo),
                Gender = GetGender(UserInfo),
                PictureUrl = GetProfilePicture()
            };

            _db.User.Add(newUser);
            _db.SaveChanges();
            return newUser;
        }

        private Person GetGoogleProfile()
        {
            ClientSecrets secrets = new ClientSecrets()
            {
                ClientId = Configuration["Authentication:Google:ClientId"],
                ClientSecret = Configuration["Authentication:Google:ClientSecret"]
            };

            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, Scopes, "user", CancellationToken.None).Result;

            var peopleService = new PeopleServiceService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Speet",
                ApiKey = "AIzaSyA6vA4EhAEPVUoWnxmsWOMbqAf_9CzOWyA"
            });

            PeopleResource.GetRequest peopleRequest = peopleService.People.Get("people/me");
            peopleRequest.PersonFields = "genders,birthdays";
            Person profile = peopleRequest.Execute();

            return profile;
        }

        private GenderType GetGender(Person user)
        {
            string genderString = user.Genders?.FirstOrDefault().Value;
            switch(genderString)
            {
                case "male":
                    return GenderType.Male;

                case "female":
                    return GenderType.Female;

                default:
                    return GenderType.Other;
            }
        }

        private DateTime GetBirthday(Person user)
        {
            DateTime birthday = Convert.ToDateTime(user.Birthdays?.FirstOrDefault()?.Text);
            return birthday;
        }

        private string GetProfilePicture()
        {
            return User.FindFirst("urn:google:picture")?.Value;
        }

        private void UpdateProfilePicture(User user)
        {
            user.PictureUrl = User.FindFirst("urn:google:picture")?.Value;
            _db.User.Update(user);
            _db.SaveChanges();
        }

        [HttpGet, Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return new RedirectResult(url: "/Site/Start", permanent: true, preserveMethod: true);
        }
    }
}
