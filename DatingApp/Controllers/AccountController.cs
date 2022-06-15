using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DatingApp.Application.Interfaces;
using DatingApp.Core.DTO;
using DatingApp.Core.Entities;
using DatingApp.Core.Interfaces;
using DatingApp.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IAccountService _accountService;
        public AccountController(DataContext context, ITokenService tokenService, IAccountService accountService)
        {
            _tokenService = tokenService;
            _context = context;
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Resgister(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) return BadRequest("This username is already in use. Please use another one");
            if (await EmailExists(registerDto.Email)) return BadRequest("This email is already in use. Please use another one");

            await _accountService.Register(registerDto);

            return Ok(new
            {
                success = true,
                message = "Your account has been created",
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {

            var loginResult = await this._accountService.Login(loginDto);
            if (loginResult.IsSuccess)
            {
                return Ok(loginResult);
            }

            else
            {
                return BadRequest(loginResult);
            }
            
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        private async Task<bool> EmailExists(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email.ToLower());
        }
    }
}
