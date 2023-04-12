using LibraryManagement.Data;
using LibraryManagement.Models.DTO;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagement.Models.Repository.Implementation
{
    public class UserRepository : Repository<User>, IUser
    {
        
        private readonly IRepository<User> _repository;
        private readonly ApiSettings _apiSettings;

        public UserRepository(LibraryContext db,IRepository<User> repository, IOptions<ApiSettings> apiSettings): base(db)
        {
            _repository = repository;
            _apiSettings = apiSettings.Value;
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            if (loginRequest == null)
            {
                throw new Exception("Invalid Login");
                
            }
            
            var user = await _repository.GetAsync(u => u.UserName == loginRequest.UserName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
            {
                return new LoginResponse()
                {
                    Token = ""
                };
            }
            
            string token = CreateToken(user);
            
            LoginResponse loginResponse = new LoginResponse()
            {
                Token = token,
            };
            return loginResponse;
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_apiSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
