﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class AppUserRole : IdentityUserRole<int>
    {

        public AppUser User { get; set; }

        public AppRole Roles { get; set; }
    }
}
