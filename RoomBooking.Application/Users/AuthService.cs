using RoomBooking.Application.Users.DTOs;
using RoomBooking.Domain.Exceptions;
using RoomBooking.Domain.Users;

namespace RoomBooking.Application.Users
{
    public sealed class AuthService
    {
        // нехай буде 3, без доп валидации
        private const int PasswordMinLength = 3;

        private readonly IUserRepository _users;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokens;

        public AuthService(IUserRepository users, IPasswordHasher passwordHasher, ITokenService tokens)
        {
            _users = users;
            _passwordHasher = passwordHasher;
            _tokens = tokens;
        }

        public async Task<AuthResponse> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken)
        {
            if (request.Password.Length < PasswordMinLength)
            {
                throw new BusinessRuleException($"Password must be at least {PasswordMinLength} characters.");
            }

            if (await _users.ExistsAsync(request.Email, cancellationToken))
            {
                throw new ConflictException("Email is already registered.");
            }

            // Реєстрація створює лише звичайного користувача
            User user = new(request.Email, _passwordHasher.Hash(request.Password), UserRole.Client);

            _users.Add(user);
            await _users.SaveChangesAsync(cancellationToken);

            return CreateResponse(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            User? user = await _users.FindByEmailAsync(request.Email, cancellationToken);

            if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            return CreateResponse(user);
        }

        private AuthResponse CreateResponse(User user)
        {
            (string token, int expiresInSeconds) = _tokens.CreateAccessToken(user);

            return new AuthResponse(token, expiresInSeconds, user.Email, user.Role.ToString());
        }

    }
}