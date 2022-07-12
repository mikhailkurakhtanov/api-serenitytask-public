using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Google.Apis.Auth;

using SerenityTask.API.Extensions;
using SerenityTask.API.Models.Client.Authentication;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Components;

namespace SerenityTask.API.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly SerenityTaskDbContext _dbContext;

        private readonly IEmailService _emailService;

        private readonly IUserService _userService;

        private readonly IGoogleIntegrationService _googleIntegrationService;

        private readonly IOptions<AuthenticationOptions> _authenticationOptions;

        public AuthenticationService(SerenityTaskDbContext dbContext, IEmailService emailService,
            IUserService userService, IGoogleIntegrationService googleIntegrationService,
            IOptions<AuthenticationOptions> authenticationOptions)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _userService = userService;
            _googleIntegrationService = googleIntegrationService;
            _authenticationOptions = authenticationOptions;
        }

        public async Task<User> RegisterUserAsync(Register formData)
        {
            var isUserExist = CheckUserOnExist(formData.Email);
            var isAccountUse = _googleIntegrationService.CheckIsAccountUse(formData.Email);

            if (isUserExist || isAccountUse) return null;

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Name = formData.Name,
                Username = formData.Username,
                Email = formData.Email,
                Role = _dbContext.Roles.Where(x => x.Id == 2).FirstOrDefault(),
                PasswordHash = formData.Password.GetPasswordHash()
            };
            _dbContext.Users.Add(newUser);

            var newUserDetails = new UserDetails
            {
                Languages = "English",
                Avatar = "assets/img/authorized/workspace/user.png",
                User = newUser
            };
            _dbContext.UserDetails.Add(newUserDetails);

            var achievementTypes = _dbContext.AchievementTypes.ToList();
            foreach (var achievementType in achievementTypes)
            {
                var newAchievement = new Achievement
                {
                    Value = 0,
                    UserDetails = newUserDetails,
                    Type = achievementType
                };

                _dbContext.Achievements.Add(newAchievement);
            }

            await _dbContext.SaveChangesAsync();

            _emailService.SendRegisterConfirmationEmail(formData);

            return newUser;
        }

        public LoginResponse LoginUser(LoginForm loginForm)
        {
            var response = new LoginResponse();
            var passwordHash = loginForm.Password.GetPasswordHash();

            var currentUser = _dbContext.Users.FirstOrDefault(x
                => x.Email == loginForm.Login || x.Username == loginForm.Login);

            if (currentUser == null) return null;

            response.IsEmailConfirmed = currentUser.IsEmailConfirmed;
            response.IsPasswordCorrect = passwordHash == currentUser.PasswordHash;

            if (response.IsEmailConfirmed && response.IsPasswordCorrect)
            {
                response.AuthorizationToken = GetJWT(currentUser);
            }

            return response;
        }

        public LoginResponse ValidateAdminData(LoginForm formData)
        {
            if (formData == null) return null;
            var response = new LoginResponse();
            var passwordHash = formData.Password.GetPasswordHash();

            var currentAdmin = _dbContext.Users.FirstOrDefault(x
                => x.Email == formData.Login && x.Role == _dbContext.Roles.Where(x => x.Id == 1).FirstOrDefault());

            if (currentAdmin == null) return null;

            response.IsPasswordCorrect = passwordHash == currentAdmin.PasswordHash;
            if (response.IsPasswordCorrect)
            {
                response.AuthorizationToken = GetJWT(currentAdmin);
            }

            return response;
        }

        public async Task<bool> CheckAuthorizedUserOnExist(Guid userId)
        {
            var existingUser = await _dbContext.Users.FindAsync(userId);
            return existingUser != null;
        }

        public bool CheckUserOnExist(string userEmail)
        {
            var existingUser = _dbContext.Users.FirstOrDefault(x => x.Email == userEmail);
            return existingUser != null;
        }

        public async Task<string> RegisterWithGoogleAsync(SocialUser data)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(data.TokenId);
            if (payload == null) return null;

            var isUserExist = CheckUserOnExist(payload.Email);
            var isAccountUse = _googleIntegrationService.CheckIsAccountUse(data.Email);
            if (isUserExist || isAccountUse) return null;

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Name = payload.GivenName,
                Username = payload.Email,
                Email = payload.Email,
                IsEmailConfirmed = true,
                IsGoogleCalendarConnected = true,
                Role = _dbContext.Roles.Where(x => x.Id == 2).FirstOrDefault(),
                PasswordHash = PasswordManager.Generate(8, 4).GetPasswordHash()
            };
            _dbContext.Users.Add(newUser);

            var newUserDetails = new UserDetails
            {
                Languages = "English",
                Avatar = string.IsNullOrWhiteSpace(payload.Picture)
                    ? "assets/img/authorized/workspace/user.png" : payload.Picture,
                User = newUser
            };
            _dbContext.UserDetails.Add(newUserDetails);

            var achievementTypes = _dbContext.AchievementTypes.ToList();
            foreach (var achievementType in achievementTypes)
            {
                var newAchievement = new Achievement
                {
                    Value = 0,
                    UserDetails = newUserDetails,
                    Type = achievementType
                };

                _dbContext.Achievements.Add(newAchievement);
            }

            var newGoogleCredentials = new GoogleCredential
            {
                Email = data.Email,
                Token = data.Token,
                TokenId = data.TokenId,
                TokenType = data.TokenDetails.TokenType,
                ExpiresIn = data.TokenDetails.ExpiresIn,
                IssuedUtc = DateTime.UtcNow,
                Scope = data.TokenDetails.Scope,
                User = newUser
            };
            newGoogleCredentials.CalendarId = await _googleIntegrationService.CreateAndGetCalendarId(newGoogleCredentials);
            _dbContext.GoogleCredentials.Add(newGoogleCredentials);

            _userService.CreateUserSettings(newUser);

            await _dbContext.SaveChangesAsync();

            var authorizationToken = GetJWT(newUser);
            return authorizationToken;
        }

        public async Task<string> LoginWithGoogleAsync(SocialUser data)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(data.TokenId);
            if (payload == null) return null;

            var existingCredental = _dbContext.GoogleCredentials.FirstOrDefault(x => x.Email == payload.Email);
            if (existingCredental == null || existingCredental.User == null) return null;

            var existingUser = existingCredental.User;
            existingUser.GoogleCredential.IssuedUtc = DateTime.UtcNow;
            existingUser.GoogleCredential.Token = data.Token;
            existingUser.GoogleCredential.TokenId = data.TokenId;
            existingUser.GoogleCredential.ExpiresIn = data.TokenDetails.ExpiresIn;

            _dbContext.Users.Update(existingUser);
            await _dbContext.SaveChangesAsync();

            var authorizationToken = GetJWT(existingUser);
            return authorizationToken;
        }

        private string GetJWT(User currentUser)
        {
            var authenticationParams = _authenticationOptions.Value;
            var securityKey = authenticationParams.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, currentUser.Id.ToString()),
                new Claim(ClaimTypes.Role, currentUser.Role.Name)
            };

            var token = new JwtSecurityToken(
                issuer: authenticationParams.Issuer,
                audience: authenticationParams.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
