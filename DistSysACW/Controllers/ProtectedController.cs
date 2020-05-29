using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DistSysACW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DistSysACW.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProtectedController : ControllerBase
    {
        // GET: api/Protected/Hello
        [Authorize(Roles = "User, Admin")]
        [ActionName("Hello")]
        [HttpGet]
        public string Hello([FromHeader(Name = "apikey")] string apikey)
        {
            User user = UserDatabaseAccess.userByKey(apikey);
            
                StatusCode(200);
                return $"Hello {user.UserName}";
        }

        // GET: api/Protected/SHA1
        [Authorize(Roles = "User, Admin")]
        [ActionName("SHA1")]
        [HttpGet]
        public string SHA1([FromQuery(Name = "message")] string input)
        {
            if (input == null)
            {
                Response.StatusCode = 400;
                return "Bad request";
            }

            byte[] inputByteArray = System.Text.Encoding.ASCII.GetBytes(input);
            SHA1 sha1provider = new SHA1CryptoServiceProvider();
            byte[] sha1ByteArray = sha1provider.ComputeHash(inputByteArray);

            string output = BitConverter.ToString(sha1ByteArray);
            output = output.Trim().Replace("-", "");
            return output;
        }

        // GET: api/Protected/SHA256
        [Authorize(Roles = "User, Admin")]
        [ActionName("SHA256")]
        [HttpGet]
        public string SHA256([FromQuery(Name = "message")] string input)
        {
            if (input == null)
            {
                Response.StatusCode = 400;
                return "Bad request";
            }

            byte[] inputByteArray = System.Text.Encoding.ASCII.GetBytes(input);
            SHA256 sha256provider = new SHA256CryptoServiceProvider();
            byte[] sha1ByteArray = sha256provider.ComputeHash(inputByteArray);

            string output = BitConverter.ToString(sha1ByteArray);
            output = output.Trim().Replace("-", "");
            return output;
        }
    }
}
