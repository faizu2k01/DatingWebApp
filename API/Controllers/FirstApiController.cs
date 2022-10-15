using API.Entities;
using API.Entities.Data;
using API.Entities.Dto;
using API.interfaces;
using AutoMapper;
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
    [Authorize]
    public class FirstApiController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public FirstApiController(IUserRepository userRepository,IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }


        [HttpGet("users")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<MemberDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<MemberDto>> Index()
        {
            var users = await _userRepository.GetMembersAsync();

           
            return Ok(users);
        }

        [HttpGet("users/{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(MemberDto),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<MemberDto>> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByUIdAsync(id);
            var userToReturn = _mapper.Map<MemberDto>(user);
            return Ok(userToReturn);
        }

        [HttpGet("users/name/{username}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(MemberDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<MemberDto>> GetUserByName(string username)
        {
            var user = await _userRepository.GetMemberByNameAsync(username);
            return Ok(user);
        }
    }
}
