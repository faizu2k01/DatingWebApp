using API.Entities.Dto;
using API.Helper;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public UserRepository(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<string> GetUserGender(string username)
        {
             return   await  _dataContext.User.Where(x => x.UserName == username).Select(x => x.Gender).FirstOrDefaultAsync();

        }

        public async Task<MemberDto> GetMemberByNameAsync(string username)
        {
            return await _dataContext.User.Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _dataContext.User.AsQueryable();

            query = query.Where(x => x.UserName != userParams.CurrentUsername );
            query = query.Where(x => x.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = query.Where(x => x.Photos.Any( x=> x.IsApproved));


            query = userParams.OrderBy switch
            {
                "created"=>query.OrderByDescending(u => u.Created),
                  _ => query.OrderByDescending(u => u.LastAction)
            };



            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(), userParams.PageNumber, userParams.PageSize);


        }

        public async Task<AppUser> GetUserByUIdAsync(int id)
        {
            return await _dataContext.User.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _dataContext.User.Include(x=>x.Photos).SingleOrDefaultAsync(x => x.UserName == username && x.Photos.Any(y => y.IsApproved && !y.IsRejected));
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _dataContext.User.Include(x => x.Photos).ToListAsync();
        }

        public void UpdateUser(AppUser user)
        {
            _dataContext.Entry(user).State = EntityState.Modified;
        }


    }
}
