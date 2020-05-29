using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DistSysACW.Controllers;
using DistSysACW.Models;
using System.Web;
using System.Collections.Generic;

namespace DistSysACW.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, Models.UserContext dbContext)
        {
            #region Task5
            // TODO:  Find if a header ‘ApiKey’ exists, and if it does, check the database to determine if the given API Key is valid
            //        Then set the correct roles for the User, using claims
            
            var requestHeaders = context.Request.Headers;

            if (requestHeaders.TryGetValue("apikey", out var keys))
            {
                string apikey = keys.ToArray()[0];

                if (UserDatabaseAccess.keyExists(apikey))
                {
                    User currentUser = UserDatabaseAccess.userByKey(apikey);

                    Claim nameClaim = new Claim(ClaimTypes.Name, currentUser.UserName);
                    Claim roleClaim = new Claim(ClaimTypes.Role, currentUser.Role);
                    Claim[] claims = new Claim[] {
                    nameClaim,
                    roleClaim };
                    ClaimsIdentity claim = new ClaimsIdentity(claims, apikey);

                    context.User.AddIdentity(claim);
                }
            }


            #endregion

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }

    }
}
