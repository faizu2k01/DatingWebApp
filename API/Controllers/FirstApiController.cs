using API.Entities;
using API.Entities.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
   
    public class FirstApiController : BaseApiController
    {
        DataContext _dataContext;
        public FirstApiController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        [HttpGet("users")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<AppUser>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> Index()
        {
            var datalist =  await _dataContext.User.ToListAsync();

            return Ok(datalist);
        }

        [HttpGet("users/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(List<AppUser>),(int)HttpStatusCode.OK)]
        public async Task<ActionResult> data(int id)
        {
            return Ok( await _dataContext.User.FindAsync(id));
        }
    }
}
