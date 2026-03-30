using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PVI.DAO.Entities.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PVI.IAM.BE.Services
{
    public class JwtGenerator
    {
        private const int EXPIRATION_MINUTES = 200;

        private readonly IConfiguration _configuration;
        private readonly GdttContext _context;


        public JwtGenerator(IConfiguration configuration, GdttContext context) : base()
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<string> CreateToken(string ma_user)
        {
            DmUser currentUser = _context.DmUsers.Where(x => x.MaUser.Equals(ma_user)).FirstOrDefault();
            if (currentUser != null)
            {
                var expiration = DateTime.UtcNow.AddMinutes(EXPIRATION_MINUTES);

                var token = CreateJwtToken(
                    await CreateClaims(currentUser),
                    CreateSigningCredentials(),
                    expiration
                );

                var tokenHandler = new JwtSecurityTokenHandler();

                return tokenHandler.WriteToken(token);
            } else
            {
                return "User " + ma_user + " không tồn tại";
            }
        }

        private async Task<Claim[]> CreateClaims(DmUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Oid.ToString()),
                //new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", user.Mail),
                new Claim(ClaimTypes.Email, user.Mail),
                new Claim(ClaimTypes.Role, ""),
                new Claim("AppId", "ff59ba95c85342a3bbdbaaf3372627f3"),
                new Claim("UserRoles", ""),
                new Claim("UserClaims", ""),            
                new Claim("MaCongTy", "INS"),
                new Claim("MaDonVi", "00"),
            };

            return claims.ToArray();
        }

        private JwtSecurityToken CreateJwtToken(Claim[] claims, SigningCredentials credentials, DateTime expiration) =>
            new JwtSecurityToken(
                _configuration["Jwt:ValidIssuer"],
                _configuration["Jwt:ValidAudience"],
                claims,
                expires: expiration,
                signingCredentials: credentials
            );

        private SigningCredentials CreateSigningCredentials() =>
            new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"])
                ),
                SecurityAlgorithms.HmacSha256
            );
    }
}