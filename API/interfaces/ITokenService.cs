using API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.interfaces
{
   public interface ITokenService
{
        public Task<string> CreateToken(AppUser appUser);
}
}
