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
    public class UserRespository : Repository<User>, IUser
    {
        private readonly LibraryContext _db;
        private readonly ApiSettings _appSettings;

        public UserRespository(LibraryContext db, IOptions<ApiSettings> appSettings): base(db)
        {
            _db = db;
            _appSettings = appSettings.Value;
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            if (loginRequest == null)
            {
                throw new Exception("Invalid Login");
                
            }
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == loginRequest.UserName);
            if (user == null)
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
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Phrase));
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
