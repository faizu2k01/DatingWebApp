
using API.interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities.Data
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DataContext _dataContext;

        public AdminRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<bool> ApproveRejectAsync(Photo photos, bool isApprove, bool isReject)
        {
            if (photos == null) return false;
            photos.IsRejected = isReject;
            photos.IsApproved = isApprove;
            _dataContext.Entry(photos).State = EntityState.Modified;
       
            

            return await _dataContext.SaveChangesAsync() > 0;

        }


      
    }
}
