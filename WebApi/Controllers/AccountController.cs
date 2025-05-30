﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTO.Account;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(JWTService jWTService, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _jwtService = jWTService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<UserDto>> RefreshUserToken()
        {
            var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
            return CreateApplicationUserDto(user);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
           // var user = await _userManager.FindByEmailAsync(loginDto.UserName);
            if (user == null)
            {
                Console.WriteLine(user.UserName);
                return Unauthorized("Invalid Username or Password");
            }

         //   return Ok(new { message = "User found", user.UserName, user.Email, user.EmailConfirmed });

            if (user.EmailConfirmed == false) return Unauthorized("Please confirm your email");

            Console.WriteLine($"Checking password for user: {user.UserName}");
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            Console.WriteLine($"Password check result: {result.Succeeded}");

            if (!result.Succeeded)
            {
                return Unauthorized("Invalid username or password");
            }

            return CreateApplicationUserDto(user);

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto register)
        {
            if (await CheckEmailExists(register.Email)){
                return BadRequest($"Account exists for {register.Email}. Please use another email");
            }

            var userToAdd = new User
            {
                FirstName = register.FirstName,
                LastName = register.LastName,
             //   Address = register.Address,
                UserName = register.Email.ToLower(),
                Email = register.Email.ToLower(),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var result = await _userManager.CreateAsync(userToAdd, register.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            else
            {
                return Ok("Your account has been created successfully.");
            }
            
        }

        #region Private Helper Methods
        private UserDto CreateApplicationUserDto(User user) {
            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                JWT = _jwtService.CreateJWT(user),

            };
        }

        private async Task<bool> CheckEmailExists(string email)
        {
            return await _userManager.Users.AnyAsync(u => u.Email == email.ToLower());
        }
        #endregion

        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
