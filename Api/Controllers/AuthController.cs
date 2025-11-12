using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Auth;
using Infrastructure.Dat;
using Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(AppDbContext db, TokenService tokenService) : ControllerBase
    {
        private readonly AppDbContext _db = db;
        private readonly TokenService _tokenService = tokenService;

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName);

            if (user == null)
                return Unauthorized("Usuário não encontrado");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Senha inválida");

            var token = _tokenService.GenerateToken(user);

            return Ok(new LoginResponse
            {
                Token = token,
                UserName = user.UserName,
                Role = user.Role.ToString()
            });
        }
    }

}


