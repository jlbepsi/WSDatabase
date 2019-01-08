using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

using EpsiLibrary2019.Model;
using EpsiLibrary2019.BusinessLogic;

namespace WSDatabase.Controllers
{
    public class DatabaseController : SecureApiController
    {
        private DatabaseService service = new DatabaseService();

        // GET: api/Database
        /// <summary>
        /// Retourne la liste des bases de données <code>DatabaseDB</code>
        /// </summary>
        /// <param name="serverId">L'identifiant du serveur de base de données</param>
        /// <returns>Une liste d'objets <code>DatabaseDB</code></returns>
        /// <example>
        /// http://serveur/api/ServerAccount/3
        /// </example>
        public List<DatabaseDB> GetDatabases()
        {
            return service.GetDatabases();
        }

        // GET: api/Database
        /// <summary>
        /// Retourne la liste des bases de données <code>DatabaseDB</code> du serveur identifié par <paramref name="serverId"/>
        /// </summary>
        /// <param name="serverId">L'identifiant du serveur de base de données</param>
        /// <returns>Une liste d'objets <code>DatabaseDB</code></returns>
        /// <example>
        /// http://serveur/api/Database/ServerId/3
        /// </example>
        [Route("api/Database/ServerId/{serverId}")]
        public List<DatabaseDB> GetDatabasesByServerId(int serverId)
        {
            return service.GetDatabasesByServerId(serverId);
        }


        // GET: api/Database
        /// <summary>
        /// Retourne la liste des bases de données <code>DatabaseDB</code> du serveur de type <paramref name="serverType"/>
        /// </summary>
        /// <param name="serverId">L'identifiant du serveur de base de données</param>
        /// <returns>Une liste d'objets <code>DatabaseDB</code></returns>
        /// <example>
        /// http://serveur/api/Database/ServerType/2
        /// </example>
        [Route("api/Database/ServerType/{serverType}")]
        public List<DatabaseDB> GetDatabasesByServerType(int serverType)
        {
            return service.GetDatabasesByServerType(serverType);
        }

        // GET: api/Database
        /// <summary>
        /// Retourne la liste des bases de données <code>DatabaseDB</code> de l'utilisateur <paramref name="userLogin"/>
        /// </summary>
        /// <param name="userLogin">L'identifiant de l'utilisateur</param>
        /// <returns>Une liste d'objets <code>DatabaseDB</code></returns>
        /// <example>
        /// http://serveur/api/Database/Login/test.v8/
        /// </example>
        [Route("api/Database/Login/{userLogin}")]
        public List<DatabaseDB> GetDatabasesByLogin(string userLogin)
        {
            return service.GetDatabasesByLogin(userLogin);
        }

        // GET: api/Database/5
        /// <summary>
        /// Retourne la base de données <code>DatabaseDB</code> identifié par <paramref name="id"/>
        /// </summary>
        /// <param name="id">L'identifiant de la base de données</param>
        /// <returns>Un objet <code>DatabaseDB</code></returns>
        /// <example>
        /// http://serveur/api/Database/3
        /// </example>
        [ResponseType(typeof(DatabaseDB))]
        public IHttpActionResult GetDatabase(int id)
        {
            DatabaseDB databaseDB = service.GetDatabase(id);
            if (databaseDB == null)
            {
                return NotFound();
            }

            return Ok(databaseDB);
        }

        // POST: api/Database
        /// <summary>
        /// Ajoute la base de données, les éléments sont identifiés par <paramref name="databaseDB"/>
        /// </summary>
        /// <param name="databaseDB">L'objet DatabaseDB a ajouter</param>
        /// <returns>Retourne l'URL de l'objet créé si l'ajout est valide, le code statut HTTP BadRequest ou Conflict sinon</returns>
        /// <example>
        /// http://serveur/api/Database/
        /// L'enveloppe Body contient le JSON de le la base de données a ajouter :
        /// <code>{ "ServerId":0,"NomBD":"DBTest2","UserLogin":"test.v8","UserNom":"V8","UserPrenom":"Test","Commentaire":"Aucun" }</code>
        /// </example>
        [ResponseType(typeof(DatabaseDB))]
        public IHttpActionResult PostDatabaseDB(DatabaseModel database)
        {
            // Vérification de l'appelant
            IHttpActionResult result = this.SecurityCheckRoleAdminOrOwner(database.UserLogin);
            if (result != null)
                return result;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DatabaseDB databaseDB = service.AddDatabase(database);
            if (databaseDB == null)
            {
                return Conflict();
            }

            return CreatedAtRoute("DefaultApi", new { id = databaseDB.Id }, databaseDB);
        }

        // PUT: api/DatabaseDBs/5
        /// <summary>
        /// Modifie les informations (commentaire, ... <paramref name="database"/>) de la base de données identifiée par <paramref name="id"/>
        /// </summary>
        /// <param name="id">L'identifiant de la base de données</param>
        /// <param name="database">L'objet DatabaseModel contenant les données</param>
        /// <returns>Retourne le code statut HTTP NoContent si la modification a été faite, BadRequest sinon</returns>
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDatabaseDB(int id, DatabaseModel database)
        {
            // Vérification de l'appelant
            IHttpActionResult result = this.SecurityCheckRoleAdminOrOwner(database.UserLogin);
            if (result != null)
                return result;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != database.ServerId)
            {
                return BadRequest();
            }

            if (! service.UpdateDatabase(id, database))
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/DatabaseDBs/5
        /// <summary>
        /// Supprime la base de données identifiée par <paramref name="id"/>
        /// </summary>
        /// <param name="id">L'identifiant de la base de données</param>
        /// <returns>Retourne le code statut HTTP Ok si la suppression a été faite, Forbidden ou NotFound sinon</returns>
        [ResponseType(typeof(DatabaseDB))]
        public IHttpActionResult DeleteDatabaseDB(int id)
        {
            // Vérification de l'appelant
            IHttpActionResult result = this.SecurityCheckRoleAdminOrUser();
            if (result != null)
                return result;

            // L'appelant doit être un administrateur de la base de données
            if (! service.IsAdministrateur(this.GetJWTIdentity().Name, id))
                return ResponseMessage(new System.Net.Http.HttpResponseMessage(HttpStatusCode.Forbidden) { ReasonPhrase = "Vous n'êtes pas administrateur de la base de données" });

            DatabaseDB databaseDB = service.RemoveDatabase(id);
            if (databaseDB == null)
                return NotFound();

            return Ok(databaseDB);
        }

        protected override void Dispose(bool disposing)
        {
            service.Dispose(disposing);
        }
    }
}