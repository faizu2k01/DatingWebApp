using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helper
{
    public class LikesParams : PaginationParmas
    {
        public int? UserId { get; set; }

        public string predicate { get; set; }
    }
}
