using API.Entities;
using System.Collections;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using API.Entities.Dto;
using API.Helper;

namespace API.interfaces
{
     public interface IUserRepository
    {
        void UpdateUser(AppUser user);

        Task<IEnumerable<AppUser>> GetUsersAsync();

        Task<AppUser> GetUserByUIdAsync(int id);

        Task<AppUser> GetUserByUsernameAsync(string username);

        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);

        Task<MemberDto> GetMemberByNameAsync(string username);

        Task<string> GetUserGender(string username);
        


    }
}
