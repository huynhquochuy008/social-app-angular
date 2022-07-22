﻿using DatingApp.Application.Interfaces;
using DatingApp.Core.Entities;
using DatingApp.Core.Extension;
using DatingApp.Core.Helpers;
using DatingApp.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly IFriendRepository _friendRepository;
        private readonly IUserRepository _userRepository;


        public FriendController(IFriendRepository friendRepository, IUserRepository userRepository)
        {
            _friendRepository = friendRepository;
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpPost("{username}")]
        public async Task<ActionResult> AddFriend(string username)
        {
            var foundedUser = await _userRepository.GetByUsername(username);
            if (foundedUser == null) return NotFound();
            var friendUser = await _friendRepository.GetUserWithLikes(foundedUser.Id);

            var sourceUserId = User.GetUserId();
            var sourceUser = await _friendRepository.GetUserWithLikes(sourceUserId); 

            var userFriend1 = new UserFriend
            {
                SourceUserId = sourceUserId,
                TargetUserId = foundedUser.Id,
            };

            var userFriend2 = new UserFriend
            {
                SourceUserId = foundedUser.Id,
                TargetUserId = sourceUserId,
            };

            sourceUser.FriendUsers.Add(userFriend1);
            friendUser.FriendUsers.Add(userFriend2);

            if (await _friendRepository.Complete()) return Ok();

            return Ok();
        }

        [Authorize]
        [HttpDelete("{username}")]
        public async Task<ActionResult> RemoveFriend(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _userRepository.GetByUsername(username);
            var sourceUser = await _friendRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();

            var userLike = await _friendRepository.GetUserLike(sourceUserId, likedUser.Id);

            sourceUser.FriendUsers.Remove(userLike);

            if (await _friendRepository.Complete()) return Ok();

            return Ok();

        }


        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<AppUser>>> GetUserLikes([FromQuery] FriendParam friendParam, int id)
        //{
        //    friendParam.UserId = User.GetUserId();
        //    var users = await _friendRepository.GetUserLikes(friendParam);

        //    return Ok(users);
        //}

        [HttpGet]
        public async Task<ActionResult> GetUserLike([FromQuery] int sourceUserId, int likedUserId)
        {
            var friend = await _friendRepository.GetUserLike(sourceUserId, likedUserId);

            return Ok(friend);
        }
    }
}
