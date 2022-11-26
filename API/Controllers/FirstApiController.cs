using API.Entities;
using API.Entities.Data;
using API.Entities.Dto;
using API.Extensions;
using API.Helper;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class FirstApiController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;

        public FirstApiController(IUnitOfWork unitOfWork,IPhotoService photoService ,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _photoService = photoService;
            _mapper = mapper;
        }


        [HttpGet("users")]
        public async Task<ActionResult<PagedList<MemberDto>>> Index([FromQuery]UserParams userParams)
        {
            var gender = await _unitOfWork.userRepository.GetUserGender(User.GetUsername());
            userParams.CurrentUsername = User.GetUsername();
            if (!string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = gender == "male" ? "female" : "male";

            var users = await _unitOfWork.userRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }

        [HttpGet("users/{id}")]
        [ProducesResponseType(typeof(MemberDto),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<MemberDto>> GetUserById(int id)
        {
            var user = await _unitOfWork.userRepository.GetUserByUIdAsync(id);
            var userToReturn = _mapper.Map<MemberDto>(user);
            return Ok(userToReturn);
        }

        [HttpGet("users/name/{username}", Name="GetUser")]
        [ProducesResponseType(typeof(MemberDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<MemberDto>> GetUserByName(string username)
        {
            var user = await _unitOfWork.userRepository.GetMemberByNameAsync(username);
            return Ok(user);
        }

        [HttpPut("update-user")]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            
            var user = await _unitOfWork.userRepository.GetUserByUsernameAsync(User.GetUsername());

            _mapper.Map(memberUpdateDto,user);

            _unitOfWork.userRepository.UpdateUser(user);


            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Failed to update user");

        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.userRepository.GetUserByUsernameAsync(User.GetUsername());

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _unitOfWork.Complete()) {

                return CreatedAtRoute("GetUser",new { username = user.UserName}, _mapper.Map<PhotoDto>(photo));
             }

            return BadRequest("Problem adding photo.");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _unitOfWork.userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");
            if (!photo.IsApproved) return BadRequest("This photo is not approved");
            

            var currentMain = user.Photos.FirstOrDefault(x=>x.IsMain);
            
            if (currentMain != null) currentMain.IsMain = false;

            photo.IsMain = true;

            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Failed to change");
                
           
        }

        [HttpDelete("delete-photos/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _unitOfWork.userRepository.GetMemberByNameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("you can not delete main photo");
            if(photo.PublicId != null)
            {
              var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("Failed to delete the photos");
        }




    }
}
