﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Core.Helpers
{
    public class LikeParam : PaginationParams
    {
        public int UserId { get; set; }
        public string Predicate { get; set; }
    }
}
