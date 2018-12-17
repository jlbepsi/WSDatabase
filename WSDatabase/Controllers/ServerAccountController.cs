﻿using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;

using EpsiLibrary2019.BusinessLogic;
using EpsiLibrary2019.Model;

namespace WSDatabase.Controllers
{
    public class ServerAccountController : ApiController
    {
        private ServerAccountService service = new ServerAccountService();

        /*// GET: api/DatabaseServerAccount
        public List<DatabaseServerUser> GetServerAccounts()
        {
            return service.Get();
        }*/

        // GET: api/ServerAccount/5
        /// <summary>
        /// Retourne la liste des comptes <code>DatabaseServerUser</code> du serveur <paramref name="serverId"/>
        /// </summary>
        /// <param name="serverId">L'identifiant du serveur de base de données</param>
        /// <returns>Une liste d'objets <code>DatabaseServerUser</code></returns>
        /// <example>
        /// http://serveur/api/ServerAccount/3
        /// </example>
        public List<DatabaseServerUser> GetServerAccounts(int serverId)
        {
            return service.GetAccounts(serverId);
        }

        // GET: api/ServerAccount/5/test.v5
        /// <summary>
        /// Retourne le compte <code>DatabaseServerUser</code> identifié par <paramref name="userLogin"/> du serveur <paramref name="serverId"/> 
        /// </summary>
        /// <param name="serverId">L'identifiant du serveur de base de données</param>
        /// <param name="userLogin">Le login C&D de l'étudiant</param>
        /// <returns>Le compte utilisateur  <code>DatabaseServerUser</code></returns>
        /// <example>
        /// http://serveur/api/ServerAccount/3/test.v5
        /// </example>
        [ResponseType(typeof(DatabaseServerUser))]
        public IHttpActionResult GetServerAccount(int serverId, string userLogin)
        {
            DatabaseServerUser databaseServerUser = service.GetAccount(serverId, userLogin);
            if (databaseServerUser == null)
            {
                return NotFound();
            }

            return Ok(databaseServerUser);
        }

        // PUT: api/DatabaseServerAccount/5
        /// <summary>
        /// Modifie le mot de passe de l'utilisateur identifié par <paramref name="serverAccount"/> sur le serveur <paramref name="serverId"/>
        /// </summary>
        /// <param name="serverId">L'identifiant du serveur de base de données</param>
        /// <param name="serverAccount">L'objet ServerAccount a modifier</param>
        /// <returns>Retourne le code statut HTTP NoContent si la modification est valide, le code statut HTTP BadRequest sinon</returns>
        /// <example>
        /// http://serveur/api/ServerAccount/3/test.v5
        /// L'enveloppe Body contient le JSON de l'utilisateur a modifier :
        /// <code>{ "ServerId"="1", "UserLogin"="test.v5", "Password"="123ABC"</code>
        /// </example>
        //[JWTAuthenticationFilter("ROLE_SUPER_ADMIN")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutServerAccount(int serverId, ServerAccount serverAccount)
        {
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

        // POST: api/DatabaseServerAccount
        /// <summary>
        /// Ajoute l'utilisateur sur le serveur, les éléments sont identifié par <paramref name="serverAccount"/>
        /// </summary>
        /// <param name="serverAccount">L'objet ServerAccount a ajouter</param>
        /// <returns>Retourne l'URL de l'objet créé si l'ajout est valide, le code statut HTTP BadRequest ou Conflict sinon</returns>
        /// <example>
        /// http://serveur/api/ServerAccount/3/test.v5
        /// L'enveloppe Body contient le JSON de l'utilisateur a ajouter :
        /// <code>{ "ServerId"="1", "UserLogin"="test.v5", "Password"="123ABC"</code>
        /// </example>
        [ResponseType(typeof(DatabaseServerUser))]
        public IHttpActionResult PostServerAccount(ServerAccount serverAccount)
        {
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

        // DELETE: api/DatabaseServerAccount/5
        /// <summary>
        /// Supprime l'utilisateur identifié par <paramref name="userLogin"/> sur le serveur <paramref name="serverId"/>
        /// </summary>
        /// <param name="serverId">L'identifiant du serveur de base de données</param>
        /// <param name="userLogin">Le login C&D de l'utilisateur a supprimer</param>
        /// <returns>Retourne le code statut HTTP Ok et le JSON de l'objet supprimé si la suppression est valide, le code statut HTTP NotFound sinon</returns>
        /// <example>
        /// http://serveur/api/ServerAccount/3/test.v5
        /// </example>
        [ResponseType(typeof(DatabaseServerUser))]
        public IHttpActionResult DeleteServerAccount(int serverId, string userLogin)
        {
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