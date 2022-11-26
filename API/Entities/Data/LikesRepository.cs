using API.Entities.Dto;
using API.Extensions;
using API.Helper;
using API.interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _dataContext;

        public LikesRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int likeUserId)
        {
            return await _dataContext.Like.FindAsync(sourceUserId, likeUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParmas)
        {
            var users = _dataContext.User.OrderBy(user => user.UserName).AsQueryable();
            var likes = _dataContext.Like.AsQueryable();

            if (likesParmas.predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId ==likesParmas.UserId);
                users = likes.Select(like => like.LikedUser);
            }


            if (likesParmas.predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likesParmas.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers =  users.Select(user => new LikeDto{
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id

            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers,likesParmas.PageNumber,likesParmas.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _dataContext.User.Include(user => user.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
