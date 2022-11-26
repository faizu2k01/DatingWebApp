using API.Entities;
using API.Entities.Dto;
using API.interfaces;
using AutoMapper;
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
    public class UserRegisterController : BaseApiController
    {
       
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public UserRegisterController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService,IMapper mapper)
        {
            
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterUserDto registerUserDto)
        {
            
            if (await isExists(registerUserDto.UserName)) return BadRequest("User already exists");

            var user = _mapper.Map<AppUser>(registerUserDto);

            user.UserName = registerUserDto.UserName.ToLower();

            var result = _userManager.CreateAsync(user,registerUserDto.Password);

            if (!result.IsCompleted) return BadRequest("Invalid Username");

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if (!roleResult.Succeeded) return BadRequest("Failed to load roles");

            return new UserDto()
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            }; 
                
        }

        private async Task<bool> isExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username);
        } 
    }
}
