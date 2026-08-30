using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RoomBooking.Application.Users;
using RoomBooking.Application.Users.DTOs;

namespace RoomBooking.Api.Controllers
{
    /// <summary>Auth сontroller</summary>
    [ApiController]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly AuthService _auth;

        public AuthController(AuthService auth)
        {
            _auth = auth;
        }

        /// <summary>Creates a client account and returns a token.</summary>
        [HttpPost("register")]
        public async Task<AuthResponse> Register(
            RegisterRequest request,
            CancellationToken cancellationToken)
        {
            return await _auth.RegisterAsync(request, cancellationToken);
        }

        /// <summary>Returns a token for an existing account</summary>
        [HttpPost("login")]
        public async Task<AuthResponse> Login(
            LoginRequest request,
            CancellationToken cancellationToken)
        {
            return await _auth.LoginAsync(request, cancellationToken);
        }

    }
}