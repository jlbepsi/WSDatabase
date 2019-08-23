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
    public class ContributorsController : SecureApiController
    {
        private DatabaseService service = new DatabaseService();

        // GET: api/Contributors/5
        /// <summary>
        /// Retourne les groupes <code>DatabaseGroupUser</code> auquel appartient le contributeur identifié par <paramref name="id"/>
        /// </summary>
        /// <param name="id">L'identifiant du contributeur</param>
        /// <returns>Une lists d'objets <code>DatabaseGroupUser</code></returns>
        /// <example>
        /// http://serveur/api/Contributors/un.contributeur.ajoute
        /// </example>
        public List<DatabaseGroupUser> Get(string id)
        {
            List<DatabaseGroupUser> list = service.GetDatabaseGroupUserWithSqlLogin(id);
            return list;
        }

        // POST: api/Contributors
        /// <summary>
        /// Ajoute un contributeur pour une base de données, les éléments sont identifiés par <paramref name="groupUserModel"/>
        /// </summary>
        /// <param name="groupUserModel">L'objet contenant les informations du contributeur et de la base de données</param>
        /// <returns>Retourne le code statut HTTP Ok si l'ajout a été fait, BadRequest ou Conflict sinon
        /// </returns>
        [JWTAuthenticationFilter]
        [ResponseType(typeof(DatabaseGroupUser))]
        public IHttpActionResult Post(GroupUserModel groupUserModel)
        {
            // Vérification de l'appelant
            IHttpActionResult result = this.SecurityCheckRoleAdminOrUser();
            if (result != null)
                return result;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // L'appelant doit être un administrateur de la base de données
            if (!service.IsAdministrateur(this.GetJWTIdentity().Name, groupUserModel.DbId))
                return ResponseMessage(new System.Net.Http.HttpResponseMessage(HttpStatusCode.Forbidden) { ReasonPhrase = "Vous n'êtes pas administrateur de la base de données" });

            DatabaseGroupUser databaseGroupUser = service.AddContributor(this.GetJWTIdentity().Name, groupUserModel);
            if (databaseGroupUser == null)
            {
                return Conflict();
            }

            //return CreatedAtRoute("DefaultApi", new { id = databaseGroupUser. }, databaseGroupUser);
            // L'utilisateur a tous les droits
            databaseGroupUser.CanBeDeleted = databaseGroupUser.CanBeUpdated = true;
            return Ok(databaseGroupUser);
        }

        // PUT: api/Contributors/5
        /// <summary>
        /// Modifie le mot de passe et/ou letype de groupe
        /// </summary>
        /// <param name="id">L'identifiant du contributeur</param>
        /// <param name="groupUserModel">L'objet contenant les informations du contributeur et de la base de données</param>
        /// <returns>Retourne le code statut HTTP Ok si la modification a été faite, BadRequest ou Conflict sinon
        [JWTAuthenticationFilter]
        [ResponseType(typeof(void))]
        public IHttpActionResult Put(String id, GroupUserModel groupUserModel)
        {
            if (!ModelState.IsValid || !id.Equals(groupUserModel.SqlLogin, StringComparison.InvariantCultureIgnoreCase))
            {
                return BadRequest(ModelState);
            }

            // Vérification de l'appelant
            IHttpActionResult result = this.SecurityCheckRoleAdminOrOwner(this.GetJWTIdentity().Name);
            if (result != null)
                return result;

            // L'appelant doit être un administrateur de la base de données
            if (! service.IsAdministrateur(this.GetJWTIdentity().Name, groupUserModel.DbId))
                return ResponseMessage(new System.Net.Http.HttpResponseMessage(HttpStatusCode.Forbidden) { ReasonPhrase = "Vous n'êtes pas administrateur de la base de données" });

            if (service.UpdateContributor(groupUserModel))
                return StatusCode(HttpStatusCode.NoContent);

            return StatusCode(HttpStatusCode.NotFound);
        }

        // DELETE: api/Contributors/5
        /// <summary>
        /// Supprime le contributeur "id" de la base de données 
        /// </summary>
        /// <param name="id">L'identifiant du contributeur</param>
        /// <param name="groupUserModel">L'objet contenant les informations du contributeur et de la base de données</param>
        /// <returns>Retourne le code statut HTTP Ok si la modification a été faite, BadRequest ou Conflict sinon
        [JWTAuthenticationFilter]
        [ResponseType(typeof(DatabaseGroupUser))]
        public IHttpActionResult Delete(String id, GroupUserModel groupUserModel)
        {
            string userLogin = this.GetJWTIdentity().Name;
            // Vérification de l'appelant
            IHttpActionResult result = this.SecurityCheckRoleAdminOrOwner(userLogin);
            if (result != null)
                return result;

            DatabaseGroupUser databaseGroupUser = service.GetDatabaseGroupUserWithSqlLogin(id, groupUserModel.DbId);
            if (databaseGroupUser == null)
            {
                return NotFound();
            }

            // L'utilisateur doit être un administrateur de la base de données
            if (! service.IsAdministrateur(userLogin, groupUserModel.DbId))
                return ResponseMessage(new System.Net.Http.HttpResponseMessage(HttpStatusCode.Forbidden) { ReasonPhrase = "Vous n'êtes pas administrateur de la base de données" });

            databaseGroupUser = service.RemoveContributor(id, groupUserModel.DbId);

            return Ok(databaseGroupUser);
        }
    }
}
