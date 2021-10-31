using Google.Apis.Auth.OAuth2;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Speet.Models;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Speet.Controllers
{
    [AllowAnonymous, Route("account")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private readonly DatabaseContext _db;

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

        public IConfiguration Configuration { get; }

        [Route("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { IsPersistent = true, RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [Route("Index")]
        public IActionResult GoogleResponse()
        {
            string googleId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (_db.User.Find(googleId) == null)
                CreateUser(googleId);

            return RedirectToAction("DiscoverGroups", "SportGroup");
        }

        private void CreateUser(string userId)
        {
            Person UserInfo = GetProfile();

            User newUser = new User()
            {
                GoogleId = userId,
                Username = User.Identity.Name,
                Birthday = BirthdayConvert(UserInfo), //DateTime.Now, //Set birthday from claim logic here
                Gender = GenderConvert(UserInfo) //GenderType.Male  //Set gender from claim logic here
            };

            _db.User.Add(newUser);
            _db.SaveChanges();
        }

        private Person GetProfile()
        {

            UserCredential credential;
            ClientSecrets secrets = new ClientSecrets()
            {
                ClientId = Configuration["Authentication:Google:ClientId"],
                ClientSecret = Configuration["Authentication:Google:ClientSecret"]
            };

            string credPath = System.Environment.GetFolderPath(
                            System.Environment.SpecialFolder.Personal);
            credPath = Path.Combine(credPath, ".credentials/people-dotnet-quickstart");

            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    secrets,
                    Scopes,
                    "me",
                    CancellationToken.None).Result;

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

        private GenderType GenderConvert(Person user)
        {
            if (user.Genders?.FirstOrDefault()?.FormattedValue == "Male")
            {
                return GenderType.Male;
            }

            if (user.Genders?.FirstOrDefault()?.FormattedValue == "Female")
            {
                return GenderType.Female;
            }

            return GenderType.Other;
        }

        private DateTime BirthdayConvert(Person user)
        {
            DateTime birthday = Convert.ToDateTime(user.Birthdays?.FirstOrDefault()?.Text);
            return birthday;
        }

        [HttpGet, Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return new RedirectResult(url: "/Site/Start", permanent: true, preserveMethod: true);
        }
    }
}
