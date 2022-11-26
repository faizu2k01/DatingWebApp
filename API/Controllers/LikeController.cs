using API.Entities;
using API.Entities.Dto;
using API.Extensions;
using API.Helper;
using API.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{   [Authorize]
    public class LikeController : BaseApiController
    {
       
        private readonly IUnitOfWork _unitOfWork;

        public LikeController(IUnitOfWork unitOfWork)
        {
           
            _unitOfWork = unitOfWork;
        }

        [HttpPost("{username}")]

        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();

            var likedUser = await _unitOfWork.userRepository.GetUserByUsernameAsync(username);

            var sourceUser = await _unitOfWork.likesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();

            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await _unitOfWork.likesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("You already like this user");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Failed to like User");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikesParams likes)
        {
            likes.UserId = User.GetUserId();
            var users = await _unitOfWork.likesRepository.GetUserLikes(likes);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }
        
    }
}
