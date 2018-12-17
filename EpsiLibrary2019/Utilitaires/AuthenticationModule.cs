using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace EpsiLibrary2019.Utilitaires
{
    public class AuthenticationModule
    {
        private const string privateKey = "<Mettre ici la clé secrète>";
        SecurityKey signingKey;
        TokenValidationParameters validationParameters;

        public AuthenticationModule()
        {
            signingKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));

            validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey
            };

        }

        public static JWTAuthenticationIdentity PopulateUserIdentity(JwtSecurityToken userPayloadToken)
        {
            string login = userPayloadToken.Claims.First(c => c.Type.Equals("sub")).Value;
            string roles = userPayloadToken.Claims.First(c => c.Type.Equals("roles")).Value;

            return new JWTAuthenticationIdentity(login, roles)
            {
                Nom = userPayloadToken.Claims.First(c => c.Type.Equals("nom")).Value,
                Prenom = userPayloadToken.Claims.First(c => c.Type.Equals("prenom")).Value,
                Mail = userPayloadToken.Claims.First(c => c.Type.Equals("mail")).Value,
                Classe = userPayloadToken.Claims.First(c => c.Type.Equals("classe")).Value
            };

        }

        public JwtSecurityToken ValidateToken(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanValidateToken)
                return null;

            ClaimsPrincipal result = null;
            JwtSecurityToken validatedToken = null;
            try
            {
                SecurityToken securityToken = null;
                result = handler.ValidateToken(accessToken, validationParameters, out securityToken);
                validatedToken = securityToken as JwtSecurityToken;
            }
            catch (Exception)
            {
            }

            return validatedToken;
        }

    }
}
