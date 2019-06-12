using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

using EpsiLibrary2019.BusinessLogic;
using EpsiLibrary2019.Model;



/*
 * IMPORTANT:
 * Pour les URL comportant un point (dans le login par exemple, 
 * il faut ajouter la ligne suivante dans le fichier web.config:
 *      <modules runAllManagedModulesForAllRequests="true">
 */

namespace WSDatabase.Controllers
{
    public class AccountsController : SecureApiController
    {
        private ServerAccountService service = new ServerAccountService();

        // GET: api/Account
        public List<DatabaseServerUser> GetServerAccounts()
        {
            return service.GetAccounts();
        }

        // GET: api/Account/5
        /// <summary>
        /// Retourne la liste des comptes <code>DatabaseServerUser</code> du serveur <paramref name="serverId"/>
        /// </summary>
        /// <param name="serverId">L'identifiant du serveur de base de données</param>
        /// <returns>Une liste d'objets <code>DatabaseServerUser</code></returns>
        /// <example>
        /// http://serveur/api/Account/byServerId/3
        /// </example>
        [Route("api/Accounts/ServerId/{serverId}")]
        public List<DatabaseServerUser> GetAccountsByServerId(int serverId)
        {
            return service.GetAccountsByServerId(serverId);
        }

        // GET: api/Account/5
        /// <summary>
        /// Retourne la liste des comptes <code>DatabaseServerUser</code> identifié par <paramref name="userLogin"/>
        /// </summary>
        /// <param name="userLogin">Le login C&D de l'étudiant</param>
        /// <returns>Une liste d'objets <code>DatabaseServerUser</code></returns>
        /// <example>
        /// http://serveur/api/Account/UserLogin/test.v8
        /// </example>
        [Route("api/Accounts/UserLogin/{userLogin}")]
        public List<ServerAccounUsertModel> GetAccountsByUserLogin(string userLogin)
        {
            return service.GetAccountsByUserLogin(userLogin);
        }

        // GET: api/Account/5
        /// <summary>
        /// Retourne la liste des comptes <code>DatabaseServerUser</code> identifié par <paramref name="sqlLogin"/>
        /// </summary>
        /// <param name="sqlLogin">Le login SQL de l'utilisateur</param>
        /// <returns>Une liste d'objets <code>DatabaseServerUser</code></returns>
        /// <example>
        /// http://serveur/api/Account/SqlLogin/test.v8/
        /// </example>
        [Route("api/Accounts/SqlLogin/{sqlLogin}")]
        public List<ServerAccounUsertModel> GetAccountsBySqlLogin(string sqlLogin)
        {
            return service.GetAccountsBySqlLogin(sqlLogin);
        }

        // GET: api/Account/5/test.v5
        /// <summary>
        /// Retourne le compte <code>DatabaseServerUser</code> identifié par <paramref name="userLogin"/> du serveur <paramref name="serverId"/> 
        /// </summary>
        /// <param name="serverId">L'identifiant du serveur de base de données</param>
        /// <param name="userLogin">Le login C&D de l'étudiant</param>
        /// <returns>Le compte utilisateur  <code>DatabaseServerUser</code></returns>
        /// <example>
        /// http://serveur/api/Account/ServerId/0/UserLogin/test.v8/
        /// </example>
        [ResponseType(typeof(DatabaseServerUser))]
        [Route("api/Accounts/ServerId/{serverId}/UserLogin/{userLogin}")]
        public IHttpActionResult GetServerAccount(int serverId, string userLogin)
        {
            DatabaseServerUser databaseServerUser = service.GetAccountByServerLogin(serverId, userLogin);
            if (databaseServerUser == null)
            {
                return NotFound();
            }

            return Ok(databaseServerUser);
        }

        // POST: api/DatabaseServerAccount
        /// <summary>
        /// Ajoute l'utilisateur sur le serveur, les éléments sont identifiés par <paramref name="serverAccount"/>
        /// </summary>
        /// <param name="serverAccount">L'objet ServerAccount a ajouter</param>
        /// <returns>Retourne l'URL de l'objet créé si l'ajout est valide, le code statut HTTP BadRequest ou Conflict sinon</returns>
        /// <example>
        /// http://serveur/api/Account/
        /// L'enveloppe Body contient le JSON de l'utilisateur a ajouter :
        /// <code>{ "ServerId"="1", "UserLogin"="test.v5", "Password"="123ABC" }</code>
        /// </example>
        [ResponseType(typeof(DatabaseServerUser))]
        public IHttpActionResult PostServerAccount(ServerAccountModel serverAccount)
        {
            // Vérification de l'appelant
            IHttpActionResult result = this.SecurityCheckRoleAdminOrOwner(serverAccount.UserLogin);
            if (result != null)
                return result;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DatabaseServerUser databaseServerUser = service.AddAccount(serverAccount);
            if (databaseServerUser == null)
            {
                return Conflict();
            }

            return CreatedAtRoute("DefaultApi", new { id = databaseServerUser.ServerId }, databaseServerUser);
        }

        // PUT: api/DatabaseServerAccount/5
        /// <summary>
        /// Modifie le mot de passe de l'utilisateur identifié par <paramref name="serverAccount"/> sur le serveur <paramref name="serverId"/>
        /// </summary>
        /// <param name="serverId">L'identifiant du serveur de base de données</param>
        /// <param name="serverAccount">L'objet ServerAccount a modifier</param>
        /// <returns>Retourne le code statut HTTP NoContent si la modification est valide, le code statut HTTP BadRequest sinon</returns>
        /// <example>
        /// http://serveur/api/Account/3/test.v5
        /// L'enveloppe Body contient le JSON de l'utilisateur a modifier :
        /// <code>{ "ServerId"="1", "UserLogin"="test.v5", "Password"="123ABC" }</code>
        /// </example>
        //[JWTAuthenticationFilter("ROLE_SUPER_ADMIN")] --> pas pour cette méthode
        //[JWTAuthenticationFilter]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutServerAccount(int serverId, ServerAccountModel serverAccount)
        {
            // Vérification de l'appelant
            IHttpActionResult result = this.SecurityCheckRoleAdminOrOwner(serverAccount.UserLogin);
            if (result != null)
                return result;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (serverId != serverAccount.ServerId)
            {
                return BadRequest();
            }

            if (service.UpdateAccount(serverAccount))
                return StatusCode(HttpStatusCode.NoContent);

            return StatusCode(HttpStatusCode.NotFound);
        }

        // DELETE: api/DatabaseServerAccount/5
        /// <summary>
        /// Supprime l'utilisateur identifié par <paramref name="userLogin"/> sur le serveur <paramref name="serverId"/>
        /// </summary>
        /// <param name="serverId">L'identifiant du serveur de base de données</param>
        /// <param name="userLogin">Le login C&D de l'utilisateur a supprimer</param>
        /// <returns>Retourne le code statut HTTP Ok et le JSON de l'objet supprimé si la suppression est valide, le code statut HTTP NotFound sinon</returns>
        /// <example>
        /// http://serveur/api/Account/3/test.v5
        /// </example>
        //[JWTAuthenticationFilter]
        [ResponseType(typeof(DatabaseServerUser))]
        public IHttpActionResult DeleteServerAccount(int serverId, string userLogin)
        {
            // Vérification de l'appelant
            IHttpActionResult result = this.SecurityCheckRoleAdminOrOwner(userLogin);
            if (result != null)
                return result;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (serverId < 0 || string.IsNullOrWhiteSpace(userLogin))
            {
                return BadRequest();
            }
            DatabaseServerUser databaseServerUser = service.RemoveAccount(serverId, userLogin);
            if (databaseServerUser == null)
            {
                return NotFound();
            }

            return Ok(databaseServerUser);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                service.Dispose(disposing);
            }
            base.Dispose(disposing);
        }
    }
}