using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

using EpsiLibrary2019.Model;
using EpsiLibrary2019.BusinessLogic;
using EpsiLibrary2019.Utilitaires;

namespace WSDatabase.Controllers
{
    public class DatabasesController : SecureApiController
    {
        private DatabaseService service = new DatabaseService();

        // GET: api/Database
        /// <summary>
        /// Retourne la liste des bases de données <code>DatabaseDB</code>
        /// </summary>
        /// <param name="serverId">L'identifiant du serveur de base de données</param>
        /// <returns>Une liste d'objets <code>DatabaseDB</code></returns>
        /// <example>
        /// http://serveur/api/databasess/3
        /// </example>
        [JWTAuthenticationFilter]
        public List<DatabaseDB> GetDatabases()
        {
            List<DatabaseDB> list = service.GetDatabases();
            FillPermissions(list, this.GetJWTIdentity());
            return list;
        }

        // GET: api/Database
        /// <summary>
        /// Retourne la liste des bases de données <code>DatabaseDB</code> du serveur identifié par <paramref name="serverId"/>
        /// </summary>
        /// <param name="serverId">L'identifiant du serveur de base de données</param>
        /// <returns>Une liste d'objets <code>DatabaseDB</code></returns>
        /// <example>
        /// http://serveur/api/databases/ServerId/3
        /// </example>
        [JWTAuthenticationFilter]
        [Route("api/Databases/ServerId/{serverId}")]
        public List<DatabaseDB> GetDatabasesByServerId(int serverId)
        {
            List<DatabaseDB> list = service.GetDatabasesByServerId(serverId);
            FillPermissions(list, this.GetJWTIdentity());
            return list;
        }


        // GET: api/Database
        /// <summary>
        /// Retourne la liste des bases de données <code>DatabaseDB</code> du serveur de type <paramref name="serverType"/>
        /// </summary>
        /// <param name="serverId">L'identifiant du serveur de base de données</param>
        /// <returns>Une liste d'objets <code>DatabaseDB</code></returns>
        /// <example>
        /// http://serveur/api/databases/ServerCode/mysql
        /// </example>
        [JWTAuthenticationFilter]
        [Route("api/Databases/ServerCode/{serverCode}")]
        public List<DatabaseDB> GetDatabasesByServerType(string serverCode)
        {
            List<DatabaseDB> list = service.GetDatabasesByServerCode(serverCode);
            FillPermissions(list, this.GetJWTIdentity());
            return list;
        }

        // GET: api/Database
        /// <summary>
        /// Retourne la liste des bases de données <code>DatabaseDB</code> de l'utilisateur <paramref name="userLogin"/>
        /// </summary>
        /// <param name="userLogin">L'identifiant de l'utilisateur</param>
        /// <returns>Une liste d'objets <code>DatabaseDB</code></returns>
        /// <example>
        /// http://serveur/api/databases/Login/test.v8/
        /// </example>
        [JWTAuthenticationFilter]
        [Route("api/Databases/Login/{userLogin}")]
        public List<DatabaseDB> GetDatabasesByLogin(string userLogin)
        {
            List<DatabaseDB> list = service.GetDatabasesByLogin(userLogin);
            FillPermissions(list, this.GetJWTIdentity());
            return list;
        }

        // GET: api/Database/5
        /// <summary>
        /// Retourne la base de données <code>DatabaseDB</code> identifié par <paramref name="id"/>
        /// </summary>
        /// <param name="id">L'identifiant de la base de données</param>
        /// <returns>Un objet <code>DatabaseDB</code></returns>
        /// <example>
        /// http://serveur/api/databases/3
        /// </example>
        [JWTAuthenticationFilter]
        [ResponseType(typeof(DatabaseDB))]
        public IHttpActionResult GetDatabase(int id)
        {
            DatabaseDB databaseDB = service.GetDatabase(id);
            if (databaseDB == null)
            {
                return NotFound();
            }

            FillPermissions(databaseDB, this.GetJWTIdentity());
            return Ok(databaseDB);
        }

        // POST: api/Database
        /// <summary>
        /// Ajoute la base de données, les éléments sont identifiés par <paramref name="databaseDB"/>
        /// </summary>
        /// <param name="databaseDB">L'objet DatabaseDB a ajouter</param>
        /// <returns>Retourne l'URL de l'objet créé si l'ajout est valide, le code statut HTTP BadRequest ou Conflict sinon</returns>
        /// <example>
        /// http://serveur/api/databases/
        /// L'enveloppe Body contient le JSON de le la base de données a ajouter :
        /// <code>{ "ServerId":0,"NomBD":"DBTest2","UserLogin":"test.v8","UserNom":"V8","UserPrenom":"Test","Commentaire":"Aucun" }</code>
        /// </example>
        [JWTAuthenticationFilter]
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

            FillPermissions(databaseDB, this.GetJWTIdentity());
            return CreatedAtRoute("DefaultApi", new { id = databaseDB.Id }, databaseDB);
        }

        // PUT: api/DatabaseDBs/5
        /// <summary>
        /// Modifie les informations (commentaire, ... <paramref name="database"/>) de la base de données identifiée par <paramref name="id"/>
        /// </summary>
        /// <param name="id">L'identifiant de la base de données</param>
        /// <param name="database">L'objet DatabaseModel contenant les données</param>
        /// <returns>Retourne le code statut HTTP NoContent si la modification a été faite, BadRequest sinon</returns>
        [JWTAuthenticationFilter]
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

            if (id != database.Id)
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
        [JWTAuthenticationFilter]
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


        /// <summary>
        /// Affecte les permissions pour chaque base de données de la liste
        /// en fonction de l'utilsateur connecté
        /// </summary>
        /// <param name="list"></param>
        /// <param name="jWTAuthenticationIdentity"></param>
        private void FillPermissions(List<DatabaseDB> list, JWTAuthenticationIdentity jwtAuthenticationIdentity)
        {
            foreach (DatabaseDB databaseDB in list)
            {
                FillPermissions(databaseDB, jwtAuthenticationIdentity);
            }
        }

        private void FillPermissions(DatabaseDB databaseDB, JWTAuthenticationIdentity jwtAuthenticationIdentity)
        {
            if (jwtAuthenticationIdentity == null || string.IsNullOrEmpty(jwtAuthenticationIdentity.Name) )
            {
                databaseDB.CanBeDeleted = databaseDB.CanBeUpdated = databaseDB.CanAddGroupUser = false;
                if (databaseDB.DatabaseGroupUsers != null)
                {
                    foreach (DatabaseGroupUser user in databaseDB.DatabaseGroupUsers)
                    {
                        user.CanBeUpdated = user.CanBeDeleted = false;
                    }
                }
            }
            else
            {
                // Si l'utilisateur est administrateur il peut faire toutes les opérations
                int groupType = DatabaseGroupUserPermissions.GetGroupType(databaseDB.DatabaseGroupUsers, jwtAuthenticationIdentity.Name);
                if (groupType == DatabaseGroupUserPermissions.ADMINISTRATEUR)
                {
                    databaseDB.CanBeDeleted = databaseDB.CanBeUpdated = databaseDB.CanAddGroupUser = true;
                    if (databaseDB.DatabaseGroupUsers != null)
                    {
                        foreach (DatabaseGroupUser user in databaseDB.DatabaseGroupUsers)
                        {
                            user.CanBeUpdated = user.CanBeDeleted = true;
                        }
                    }
                }
                else
                {
                    databaseDB.CanBeDeleted = databaseDB.CanBeUpdated = databaseDB.CanAddGroupUser = false;
                    if (databaseDB.DatabaseGroupUsers != null)
                    {
                        foreach (DatabaseGroupUser user in databaseDB.DatabaseGroupUsers)
                        {
                            // Si l'utilisateur connecté est l'utilisateur alors il peut faire les actions
                            if (!String.IsNullOrWhiteSpace(user.UserLogin) &&  user.UserLogin.Equals(jwtAuthenticationIdentity.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                user.CanBeUpdated = user.CanBeDeleted = true;
                            }
                            else
                            {
                                user.CanBeUpdated = user.CanBeDeleted = false;
                            }
                        }
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            service.Dispose(disposing);
        }
    }
}