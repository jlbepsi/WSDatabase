using EpsiLibrary2019.Utilitaires;
using System.Net;
using System.Security.Principal;
using System.Web.Http;

namespace WSDatabase.Controllers
{
    public class SecureApiController : ApiController
    {
        protected IHttpActionResult SecurityCheckRoleAdminOrUser()
        {
            if (this.RequestContext.Principal == null || this.RequestContext.Principal.Identity == null)
                return StatusCode(HttpStatusCode.Forbidden);

            /* La demande de modification est valide ssi 
             *  - le rôle est ROLE_SUPER_ADMIN
             *  ou 
             *  - le rôle est ROLE_USER
             */
            if (!(this.RequestContext.Principal.IsInRole("ROLE_SUPER_ADMIN") || this.RequestContext.Principal.IsInRole("ROLE_USER")))
            {
                // return StatusCode(HttpStatusCode.Forbidden);
                return ResponseMessage(new System.Net.Http.HttpResponseMessage(HttpStatusCode.Forbidden) { ReasonPhrase = "Vous n'êtes pas le propriétaire du compte" });
            }

            return null;
        }

        protected IHttpActionResult SecurityCheckRoleAdminOrOwner(string userLogin)
        {
            if (this.RequestContext.Principal == null || this.RequestContext.Principal.Identity == null)
                return StatusCode(HttpStatusCode.Forbidden);

            /* La demande de modification est valide ssi 
             *  - le rôle est ROLE_SUPER_ADMIN
             *  ou 
             *  - le rôle est ROLE_USER et l'utilisateur authentifié pat le token est l'utilisateur qui fait la modification
             */
            if (! (this.RequestContext.Principal.IsInRole("ROLE_SUPER_ADMIN") ||
                  (this.RequestContext.Principal.IsInRole("ROLE_USER") && this.RequestContext.Principal.Identity.Name.Equals(userLogin, System.StringComparison.InvariantCultureIgnoreCase))))
            {
                // return StatusCode(HttpStatusCode.Forbidden);
                return ResponseMessage(new System.Net.Http.HttpResponseMessage(HttpStatusCode.Forbidden) { ReasonPhrase = "Vous n'êtes pas le propriétaire du compte" });
            }

            return null;
        }

        protected JWTAuthenticationIdentity GetJWTIdentity()
        {
            if (this.RequestContext.Principal == null)
                return null;

            return this.RequestContext.Principal.Identity as JWTAuthenticationIdentity;
        }
    }
}