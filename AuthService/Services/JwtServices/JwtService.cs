using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthService.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services.JwtServices
{
    public class JwtService :IJwtService
    {
        private readonly SignInManager<User> _signInManager;
        public JwtService(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
           
        }
        public async Task<string> GenerateAsync(User user)
        {
            var secretKey = "mdlkvnkjnmkFEFmfk vi infnojFEFn#$#$   kmffs',rkEFEFisrsd_jn3*^";
            var encryptionkey = "17COaqEn&rLptMey";
            var claims = await GetClaimsAsync(user);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),SecurityAlgorithms.HmacSha256Signature);
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptionkey)), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);
            var descriptor = new SecurityTokenDescriptor()
            {
                Audience = "Audience",
                Issuer = "Issuer",
                IssuedAt = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(340),
                NotBefore = DateTime.Now.AddMinutes(0),
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials
            };
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);
            var jwt = tokenHandler.WriteToken(securityToken);
            return jwt;
        }

        private async Task<IEnumerable<Claim>> GetClaimsAsync(User user)
        {
            var result = await _signInManager.ClaimsFactory.CreateAsync(user);
            return result.Claims;
           
        }
    }

    
}
