﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Application.DTO
{
    public class ProfileDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string Phone { get; set; }

        public IFormFile Avatar { get; set; }
        public IFormFile CoverPhoto { get; set; }
    }
}
