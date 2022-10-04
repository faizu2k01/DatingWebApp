using API.Entities;
using API.Entities.Data;
using API.Entities.Dto;
using API.interfaces;
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
        DataContext _dataContext;
        private readonly ITokenService _tokenService;

        public UserRegisterController(DataContext dataContext, ITokenService tokenService)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterUserDto registerUserDto)
        {
            
            if (await isExists(registerUserDto.UserName)) return BadRequest("User already exists");
           
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerUserDto.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerUserDto.Password)),
                PasswordSalt = hmac.Key
            };

            _dataContext.User.Add(user);
            await _dataContext.SaveChangesAsync();

            return new UserDto()
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            }; 
                
        }

        private async Task<bool> isExists(string username)
        {
            return await _dataContext.User.AnyAsync(x => x.UserName == username);
        } 
    }
}
