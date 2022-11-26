using API.Entities;
using API.Entities.Data;
using API.Entities.Dto;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(UserManager<AppUser> userManager,IMapper mapper , DataContext dataContext,IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            
            _mapper = mapper;
            _dataContext = dataContext;
            _unitOfWork = unitOfWork;
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
                 .Include(u => u.UserRoles)
                 .ThenInclude(r => r.Roles)
                 .OrderBy(r => r.UserName)
                 .Select(user => new 
                 {
                     user.Id,
                     UserName = user.UserName,
                     Roles = user.UserRoles.Select(u => u.Roles.Name).ToList()

                 }).ToListAsync();

            return Ok(users);
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add role");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to fetch user role");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy ="ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult<List<UsersPhotoDto>>> GetPhotosForModeration()
        {
            var list = await _userManager.Users.Where(x => x.Photos.Any(x => !string.IsNullOrEmpty(x.PublicId) && !x.IsApproved && x.IsRejected && !x.IsMain) )
                .Include(x => x.Photos)
                .Select( x => new UsersPhotoDto
                {
                    UserId = x.Id,
                    UserName = x.UserName,
                    Country = x.Country,
                    PhotoUrl = x.Photos.Where(x => !string.IsNullOrEmpty(x.PublicId) && x.IsRejected && !x.IsMain).Select(x => x.Url).FirstOrDefault(),
                    PublicId = x.Photos.Where(x => !string.IsNullOrEmpty(x.PublicId) &&  x.IsRejected && !x.IsMain).Select(x => x.PublicId).FirstOrDefault()

                })
                .ToListAsync();

            if (list == null || list.Count() <= 0) return new List<UsersPhotoDto>();

            return  Ok(list);
        }


        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPost("approved-user-photo")]
        public async Task<ActionResult<bool>> ApproveUserPhoto([FromQuery] int userId,string username,string publicId,bool isApprove,bool isRejected )
        {
            
            var user = await _userManager.Users.Where(x =>x.Id == userId &&  x.UserName.ToLower() == username && x.Photos.Any(x => x.PublicId == publicId))
                .Include(x => x.Photos)
                .Select(x => x.Photos.FirstOrDefault(y => y.PublicId == publicId))
                .FirstOrDefaultAsync();

            if (user == null) return BadRequest("Not having user");
                
            return Ok( await _unitOfWork.adminRepository.ApproveRejectAsync(user,isApprove,isRejected));
        }

    }
}
