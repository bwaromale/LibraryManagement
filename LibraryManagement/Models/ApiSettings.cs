using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LibraryManagement.Models
{
    public class ApiSettings
    {
        //private readonly IConfiguration _configuration;

        //public ApiSettings(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        public string SecretKey { get; set; }


        //public TokenValidationParameters GetTokenValidationParameters()
        //{
        //    return new TokenValidationParameters
        //    {
        //        ValidateIssuer = false,
        //        ValidateAudience = false,
        //        ValidateLifetime = true,
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("ApiSettings:SecretKey").Value))
        //    };
        //}
    }
}
