using DistSysACW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text;

namespace DistSysACW.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // GET: api/User/New?
        [ActionName("New")]
        public IActionResult Get([FromQuery]string username)
        {
            if (UserDatabaseAccess.userByName(username).UserName != "")
            {
                return Ok("True - User Does Exist! Did you mean to do a POST to create a new user?");
            }
            else
            {
                return Ok("False - User Does Not Exist! Did you mean to do a POST to create a new user?");
            }
        }

        // POST: api/User/New
        [ActionName("New")]
        [HttpPost]
        public IActionResult Post([FromBody] string username)
        {
            if (username == "" || username == null)
            {
                return BadRequest("Oops. Make sure your body contains a string with your username and your Content - Type is Content - Type:application / json");
            }
            else if (UserDatabaseAccess.usernameExists(username))
            {
                var responseMessage = "Oops. This username is already in use. Please try again with a new username.";
                byte[] data = Encoding.UTF8.GetBytes(responseMessage);

                Response.StatusCode = 403;
                Response.Body.WriteAsync(data);
                return StatusCode(403); // Not sure if need, left in since it returns
            }
            else
            {
                User user = UserDatabaseAccess.createUser(username);
                return Ok(user.ApiKey);
            }
        }

        // DELETE: api/User/RemoveUser
        [Authorize(Roles = "User, Admin")]
        [ActionName("RemoveUser")]
        [HttpDelete]
        public bool Delete([FromQuery] string username, [FromHeader(Name = "apikey")] string apikey)
        {
            if (UserDatabaseAccess.userExists(username, apikey))
            {
                return UserDatabaseAccess.deleteByKey(apikey);
            }
            else
                return false;
        }

        // POST: api/User/Changerole
        [Authorize(Roles = "Admin")]
        [ActionName("ChangeRole")]
        [HttpPost]
        public IActionResult ChangeRole([FromBody] UserJSON user, [FromHeader(Name = "apikey")] string apikey)
        {
            if (!UserDatabaseAccess.keyExists(apikey))
                return BadRequest("NOT DONE: Apikey does not exist");

            if (!UserDatabaseAccess.usernameExists(user.username))
                return BadRequest("NOT DONE: Username does not exist");

            List<string> roleList = new List<string>{
                    "admin",
                    "user"};

            if (!roleList.Contains(user.role.ToLower()))
                return BadRequest("NOT DONE: Role does not exist");

            if (UserDatabaseAccess.changeRole(user.role, user.username))
                return Ok("DONE");


            return BadRequest("NOT DONE: An error occurred");
        }
    }
}
