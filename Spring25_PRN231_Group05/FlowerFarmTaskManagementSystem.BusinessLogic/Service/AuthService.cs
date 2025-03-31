using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.Service
{
	public class AuthService : IAuthService
	{
		private readonly IConfiguration configuration;
		private IUnitOfWork _unitOfWork;
		//private IPasswordHasher<User> _passwordHasher;

		public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
		{
			this.configuration = configuration;
			_unitOfWork = unitOfWork;
			//_passwordHasher = passwordHasher;
		}

		public async Task<AuthResponseDTO> LoginAsync(AuthRequestDTO request)
		{
			var user = await _unitOfWork.UserRepository.GetAsync(x => x.Email == request.UserNameOrEmail || x.UserName == request.UserNameOrEmail);

			if (user == null)
			{
				// Optionally log the failed login attempt
				return null; // or throw an exception
			}

			//var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
			//// Verify the password (assuming you have a method to check the password)
			//if (passwordVerificationResult != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
			//{
			//	return null; // or throw an exception for incorrect password
			//}

			if (user.Password != request.Password)
			{
				return null;
			}

			var token = CreateToken(user);
			return new AuthResponseDTO
			{
				Username = user.UserName,
				Token = token,
				Email = user.Email, // Use the user's email instead of request.Email
			};
		}
		private string CreateToken(User user)
		{
			List<Claim> claims = new List<Claim>
			{
				new Claim("UserId", user.UserId.ToString()),
				//new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
				new Claim("Role", user.Role.ToString()),
				//new Claim(ClaimTypes.Role, user.Role),
				//new Claim( "Email" , user.Email ?? string.Empty),
				//new Claim(ClaimTypes.Email, user.Email),
				new Claim("Username", user.UserName ?? string.Empty),
				//new Claim(ClaimTypes.Name, user.UserName),
			};

			var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.Now.AddDays(1),
				signingCredentials: creds);

			var jwt = new JwtSecurityTokenHandler().WriteToken(token);
			return jwt;
		}
	}
}
