using Google.Apis.Auth.AspNetCore3;
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
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Speet.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly DatabaseContext _db;
        public IConfiguration Configuration { get; }
        public IGoogleAuthProvider _auth;

        static string[] Scopes = {
                "profile",
                "https://www.googleapis.com/auth/user.gender.read",
                "https://www.googleapis.com/auth/user.birthday.read"
            };

        public AccountController(DatabaseContext db, IConfiguration configuration, IGoogleAuthProvider auth)
        {
            _db = db;
            Configuration = configuration;
            _auth = auth;
        }

        [Authorize]
        [Route("google-login")]
        public IActionResult GoogleLogin()
        {
            //var properties = new AuthenticationProperties { IsPersistent = true, RedirectUri = Url.Action("GoogleResponse") };
            //return Challenge(properties, GoogleDefaults.AuthenticationScheme);
            return RedirectToAction("GoogleResponse", "Account");
        }

        public IActionResult GoogleResponse()
        {
            string googleId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            User user = _db.User.Find(googleId);
            var UserClaims = User.Claims.ToList();

            if (user == null)
                user = CreateUser(googleId);

            if (user.PictureUrl != UserClaims[4].Value)
                UpdateProfilePicture(user);

            return RedirectToAction("DiscoverGroups", "SportGroup");
        }

        private User CreateUser(string userId)
        {
            Person UserInfo = GetGoogleProfile();

            var UserClaims = User.Claims.ToList();

            User newUser = new User()
            {
                GoogleId = userId,
                Username = UserClaims[3].Value,
                Birthday = GetBirthday(UserInfo),
                Gender = GetGender(UserInfo),
                PictureUrl = UserClaims[4].Value
            };

            _db.User.Add(newUser);
            _db.SaveChanges();
            return newUser;
        }

        private Person GetGoogleProfile()
        {

            //UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, Scopes, "user", CancellationToken.None).Result;
            GoogleCredential cred = _auth.GetCredentialAsync().Result;


            var peopleService = new PeopleServiceService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = cred,
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

        private void UpdateProfilePicture(User user)
        {
            var UserClaims = User.Claims.ToList();
            user.PictureUrl = UserClaims[3].Value;
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
