using API.Entities;
using API.Entities.Dto;
using API.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.interfaces
{
   public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likeUserId);

        Task<AppUser> GetUserWithLikes(int userId);

        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likes);

    }
}
