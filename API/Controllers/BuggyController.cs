using API.Entities;
using API.Entities.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;
        public BuggyController(DataContext context)
        {
            _context = context;

        }
       

        [HttpGet("auth")]
        [Authorize]
        public ActionResult<string> GetSecret()
        {
            return "this is secret";
        }



        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = _context.User.Find(-1);
            if (thing == null) return NotFound();

            return Ok(thing);
        }


        [HttpGet("server-error")]

        public ActionResult<string> GetSeverError()
        {
            var thing = _context.User.Find(-1);

            var thingToReturn = thing.ToString();

            return thingToReturn;
        }

        [HttpGet("bad-request")]
    
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("Bad request");
        }






    }

}
