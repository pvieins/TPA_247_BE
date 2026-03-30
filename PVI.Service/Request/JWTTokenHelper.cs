using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Claims;

namespace PVI.Service.Request
{
    public static class JwtTokenHelper
    {
        public static string ExtractTokenInfoAndSetParameters(HttpContext httpContext)
        {
            var tokenHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (tokenHeader != null && tokenHeader.StartsWith("Bearer "))
            {
                var token = tokenHeader.Substring("Bearer ".Length).Trim();
                
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                 var maUser1 = jwtToken.Claims.FirstOrDefault(c => c.Type == "username")?.Value;

                return maUser1;
            }
            return null;
        }
        public static string ExtractTokenInfoAndSetEmail(HttpContext httpContext)
        {
            var tokenHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (tokenHeader != null && tokenHeader.StartsWith("Bearer "))
            {
                var token = tokenHeader.Substring("Bearer ".Length).Trim();

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                var emailaddress = jwtToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;



                return emailaddress;
            }
            return null;
        }




        public static string ExtractTokenInfoAndSetInfo(HttpContext httpContext,string type)
        {
            var info = "";
            var tokenHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (tokenHeader != null && tokenHeader.StartsWith("Bearer "))
            {
                var token = tokenHeader.Substring("Bearer ".Length).Trim();

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                 info = jwtToken.Claims.FirstOrDefault(c => c.Type == type)?.Value;



                return info;
            }
            return null;
        }

        public static string Generate_JWT_Token_From_Email(string currentUserEmail)
        {
            try
            {
                byte[] symmetricKey = Convert.FromBase64String("TESTKEYSTESTKEYSTESTKEYS");
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                DateTime now = DateTime.UtcNow;
                SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                            {
                            new Claim(ClaimTypes.Email, currentUserEmail),
                        }),
                    Claims = new Dictionary<string, object>
                {
                    { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", currentUserEmail},
                        
                },
                    Expires = now.AddMinutes(60),
                    //Expires = now.AddSeconds(Convert.ToInt32(EXPIRE_IN_MINUTE)),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
                };

                SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
                string token = tokenHandler.WriteToken(securityToken);
                return token;
            }
            catch (Exception ex)
            {
                return "";
            }
        }


    }
}
