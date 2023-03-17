using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LibraryManagement.Models
{
    public class ApiSettings
    {
        public string Phrase { get; set; }
        //public string Secret { get; set; }
        //public string JwtIssuer { get; set; } = "shawnlibrary.com";
        //public string JwtAudience { get; set; } = "shawnlibrary.com";
        //public string JwtSecretKey { get; set; } = "shawnsecret";

        //public TokenValidationParameters GetTokenValidationParameters()
        //{
        //    return new TokenValidationParameters
        //    {
        //        ValidateIssuer = true,
        //        ValidateAudience = true,
        //        ValidateLifetime = true,
        //        ValidateIssuerSigningKey = true,
        //        ValidIssuer = JwtIssuer,
        //        ValidAudience = JwtAudience,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtSecretKey))
        //    };
        //}
    }
}
