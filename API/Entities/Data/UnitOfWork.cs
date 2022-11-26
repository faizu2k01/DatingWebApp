using API.interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public UnitOfWork(DataContext dataContext,IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public IUserRepository userRepository => new UserRepository(_dataContext,_mapper);

        public IMessageRepository messageRepository => new MessageRepository(_dataContext,_mapper);

        public ILikesRepository likesRepository => new LikesRepository(_dataContext);

        public IAdminRepository adminRepository => new AdminRepository(_dataContext);

        public async Task<bool> Complete()
        {
            return await _dataContext.SaveChangesAsync() > 0; 
        }

        
        public bool HasChange()
        {   //this method return true if there is some changes that require save otherwise return false;
            return _dataContext.ChangeTracker.HasChanges();
        }
    }
}
