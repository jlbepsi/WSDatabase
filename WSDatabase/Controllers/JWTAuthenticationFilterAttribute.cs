using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

using EpsiLibrary2019.Utilitaires;
using System.Security.Principal;

namespace WSDatabase.Controllers
{
    public class JWTAuthenticationFilterAttribute : AuthorizationFilterAttribute
    {
        private string role;

        public JWTAuthenticationFilterAttribute()
        {
        }
        public JWTAuthenticationFilterAttribute(string role)
        {
            this.role = role;
        }

        public override void OnAuthorization(HttpActionContext filterContext)
        {
            if (!IsUserAuthorized(filterContext))
            {
                filterContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                return;
            }
            base.OnAuthorization(filterContext);
        }

        private bool IsUserAuthorized(HttpActionContext actionContext)
        {
            string token = FetchFromHeader(actionContext); // fetch authorization token from header


            if (token != null && !String.IsNullOrWhiteSpace(token))
            {
                AuthenticationModule auth = new AuthenticationModule();
                JwtSecurityToken userPayloadToken = auth.ValidateToken(token);

                if (userPayloadToken != null)
                {
                    JWTAuthenticationIdentity identity = AuthenticationModule.PopulateUserIdentity(userPayloadToken);

                    if (this.role == null || identity.Roles.Contains(this.role))
                    {
                        actionContext.ControllerContext.RequestContext.Principal = identity.GetPrincipal();
                        return true;
                    }
                }
            }
            return false;
        }

        private string FetchFromHeader(HttpActionContext actionContext)
        {
            string requestToken = null;

            var authRequest = actionContext.Request.Headers.Authorization;
            if (authRequest != null)
            {
                requestToken = authRequest.Parameter;
            }

            return requestToken;
        }
    }
}