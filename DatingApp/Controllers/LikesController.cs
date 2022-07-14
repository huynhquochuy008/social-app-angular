﻿using DatingApp.Application.DTO.Likes;
using DatingApp.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly ILikeService _likeService;

        public LikesController(ILikeService likeService)
        {
            _likeService = likeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewLike([FromBody] AddLikeRequest request)
        {
            try
            {
                var newLike = await _likeService.CreateNewLike(request);
                return Ok(newLike);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateLikeStatus([FromBody] UpdateLikeRequest request)
        {
            var updatedLike = await _likeService.UpdateLike(request);
            return Ok(updatedLike);
        }       
    }
}
