using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ServerDemo.Controllers
{
    [Route("api/[controller]")]
    public class ServerDateTimeController : Controller
    {
        // GET api/ServerDateTime
        [HttpGet]
        [Authorize(Policy = "IsValidClientUser")]
        public string Get()
        {
            return DateTime.Now.ToString("MM-dd-yyyy HH:mm:sss");
        }
    }
}
