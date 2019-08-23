using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.IO;

namespace EpsiLibrary2019.Utilitaires
{
    public class AuthenticationModule
    {
        //private const string privateKey = "<Mettre ici la clé secrète>";
        TokenValidationParameters validationParameters;

        public AuthenticationModule()
        {
            //SecurityKey signingKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));

            var xmlString = String.Empty;
            var path = ConfigurationManager.GetConfigurationManager().GetValue("security.keys.public");
            X509Certificate2 x509Certificate = LoadCertificateFile(path);
            X509SecurityKey publicKey = new X509SecurityKey(x509Certificate);

            validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = publicKey
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
 
        private static X509Certificate2 LoadCertificateFile(string filename)
        {
            X509Certificate2 x509 = null;
            using (FileStream fs = File.OpenRead(filename))
            {
                byte[] rawData = new byte[fs.Length];
                fs.Read(rawData, 0, rawData.Length);
                if (rawData != null)
                {
                    x509 = new X509Certificate2(rawData);
                    /*x509 = new X509Certificate2();
                    x509.Import(rawData);*/
                }
            }
            return x509;
        }

    }
}
