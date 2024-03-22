using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Blog.Models;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Services
{
    public class TokenService
    {
        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // Chave transformada em array de bytes
            var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
            // Configurações do token | Contém todas as infos do token
            var tokenDescriptor = new SecurityTokenDescriptor();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
