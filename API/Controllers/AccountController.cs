using API.Entities;
using API.Entities.Data;
using API.Entities.Dto;
using API.interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _token;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,  ITokenService token)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _token = token;
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> LoginUser(UserLoginDto registerUserDto)
        {
            registerUserDto.UserName = registerUserDto.UserName.ToLower();
            var user = await _userManager.Users.Include(x=>x.Photos).SingleOrDefaultAsync(x => x.UserName == registerUserDto.UserName);

            if (user == null) return Unauthorized("User not found");

            var result = await _signInManager.CheckPasswordSignInAsync(user, registerUserDto.Password, false);

            if (!result.Succeeded) return Unauthorized("Invalid password");
            return new UserDto()
            {
                UserName = user.UserName,
                Token = await _token.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                Gender = user.Gender
            };
        }

        
    }
}
