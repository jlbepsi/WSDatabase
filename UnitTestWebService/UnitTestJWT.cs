using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using EpsiLibrary2019.Utilitaires;

namespace UnitTestWebService
{
    [TestClass]
    public class UnitTestJWT
    {
        [TestMethod]
        public void TokenValidation()
        {
            AuthenticationModule authentication = new AuthenticationModule();
            string token = "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJhZG1pbi50ZXN0Iiwibm9tIjoiQWRtaW5pc3RyYXRldXIiLCJwcmVub20iOiJBZG1pbmlzdHJhdGV1ciIsIm1haWwiOiJhZG1pbi5yZXNlYXVAbW9udHBlbGxpZXItZXBzaS5mciIsImNsYXNzZSI6IkFkbWluaXN0cmF0aW9uIiwicm9sZXMiOiJST0xFX1NVUEVSX0FETUlOIiwiaWF0IjoxNTQ0Nzc3NzgzLCJleHAiOjE1NDQ3ODQ5ODN9.ebNHIHnaOtiCTPJmP2a0V7vhkrCZB0S5-wpN2fkzOKk";
            JwtSecurityToken securityToken = authentication.ValidateToken(token);

            Assert.IsNotNull(securityToken);

            JWTAuthenticationIdentity identity = AuthenticationModule.PopulateUserIdentity(securityToken);

            Assert.IsTrue(identity.Name.Equals("admin.test"));
            Assert.IsTrue(identity.Nom.Equals("Administrateur"));
            Assert.IsTrue(identity.Mail.Equals("admin.reseau@montpellier-epsi.fr"));
            Assert.IsTrue(identity.Roles.Contains("ROLE_SUPER_ADMIN"));
        }
    }
}
