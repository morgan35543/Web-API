using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DistSysACW.Controllers
{
    public class TalkBackController : BaseController
    {
        /// <summary>
        /// Constructs a TalkBack controller, taking the UserContext through dependency injection
        /// </summary>
        /// <param name="context">DbContext set as a service in Startup.cs and dependency injected</param>
        public TalkBackController(Models.UserContext context) : base(context) { }


        [ActionName("Hello")]
        [HttpGet]
        public IActionResult Get()
        {
            #region TASK1
            // TODO: add api/talkback/hello response

            return Ok("Hello world");

            #endregion
        }

        [ActionName("Sort")]
        public IActionResult Get([FromQuery]int[] integers) // Responds 400 Bad request if something isn't an int
        {
            #region TASK1
            // TODO: 
            // sort the integers into ascending order
            // send the integers back as the api/talkback/sort response                       

            try
            {
                List<int> intList = integers.OfType<int>().ToList();

                intList.Sort();

                return Ok(intList);
            }
            catch (Exception e)
            {
                return BadRequest("400 Bad Request");
            }

            #endregion
        }
    }
}
