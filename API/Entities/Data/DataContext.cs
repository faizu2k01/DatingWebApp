using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
                
        }


       public DbSet<AppUser> User { get; set; }

        public DbSet<Photo> Photo { get; set; }
    }
}
