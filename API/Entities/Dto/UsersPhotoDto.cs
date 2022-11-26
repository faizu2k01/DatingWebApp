using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities.Dto
{
    public class UsersPhotoDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        public string Country { get; set; }

        public string  PhotoUrl { get; set; }

        public string PublicId { get; set; }

        
    }
}
