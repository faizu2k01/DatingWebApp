
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.interfaces
{
   public interface IUnitOfWork
    {
        IUserRepository userRepository { get; }

        IMessageRepository messageRepository { get; }

        ILikesRepository likesRepository { get; }

        IAdminRepository adminRepository { get; }

        Task<bool> Complete();

        bool HasChange();
    }
}
