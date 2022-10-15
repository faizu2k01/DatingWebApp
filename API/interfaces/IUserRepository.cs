using API.Entities;
using System.Collections;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using API.Entities.Dto;

namespace API.interfaces
{
     public interface IUserRepository
    {
        void UpdateUser(AppUser user);

        Task<bool> SaveAllAsync();

        Task<IEnumerable<AppUser>> GetUsersAsync();

        Task<AppUser> GetUserByUIdAsync(int id);

        Task<AppUser> GetUserByUsernameAsync(string username);

        Task<IEnumerable<MemberDto>> GetMembersAsync();

        Task<MemberDto> GetMemberByNameAsync(string username);
        


    }
}
