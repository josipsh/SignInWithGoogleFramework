using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;

namespace SignInWithGoogleFramework.Controllers
{
    public class AuthController : ApiController
    {
        // GET api/values
        [HttpPost]
        public Object GetToken([FromBody] string data)
        {
            try{

                GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();

                settings.Audience = new List<string>()
                    {
                        "751388532460-p2l56eq2plddt99f0n352gutc1k796t1.apps.googleusercontent.com"
                    };

                GoogleJsonWebSignature.Payload payload = GoogleJsonWebSignature.ValidateAsync(data, settings).Result;

                string key = "ThisIsMyToken123_IHopeThisWillBeLongEnough";
                string issuer = "Josip";

                SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                IList<Claim> permClaim = new List<Claim>();
                permClaim.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                permClaim.Add(new Claim("valid", "1"));
                permClaim.Add(new Claim("userId", "1"));
                permClaim.Add(new Claim("name", payload.Email));

                JwtSecurityToken token = new JwtSecurityToken(
                    issuer, 
                    issuer, 
                    permClaim, 
                    expires: DateTime.Now.AddDays(1), 
                    signingCredentials: credentials
                );

                string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                return new { data = jwtToken };
            }catch(Exception e)
            {
                return e.Message;
            }
        }
    }
}
