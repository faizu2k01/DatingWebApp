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
    public class AccountController : BaseApiController
    {
        DataContext _dataContext;
        private readonly ITokenService _token;

        public AccountController(DataContext dataContext,ITokenService token)
        {
            _dataContext = dataContext;
            _token = token;
        }
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> LoginUser(RegisterUserDto registerUserDto)
        {
            var user = await _dataContext.User.SingleOrDefaultAsync(x => x.UserName == registerUserDto.UserName);
            if (user == null) return Unauthorized("User not found");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerUserDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
               if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Wrong password");
            }

            return new UserDto()
            {
                UserName = user.UserName,
                Token = _token.CreateToken(user)
            };
        }
    }
}
