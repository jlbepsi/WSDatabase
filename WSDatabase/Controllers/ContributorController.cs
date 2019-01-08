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
    public class ContributorController : SecureApiController
    {
        private DatabaseService service = new DatabaseService();

        // GET: api/Contributor/5
        /// <summary>
        /// Retourne les groupes <code>DatabaseGroupUser</code> auquel appartient le contributeur identifié par <paramref name="sqlLogin"/>
        /// </summary>
        /// <param name="id">L'identifiant de la base de données</param>
        /// <returns>Un objet <code>DatabaseDB</code></returns>
        /// <example>
        /// http://serveur/api/Contributor/un.contributeur.ajoute
        /// </example>
        public List<DatabaseGroupUser> Get(string sqlLogin)
        {
            List<DatabaseGroupUser> list = service.GetDatabaseGroupUserWithSqlLogin(sqlLogin);
            return list;
        }

        // POST: api/Contributor
        /// <summary>
        /// Ajoute un contributeur pour une base de données, les éléments sont identifiés par <paramref name="groupUserModel"/>
        /// </summary>
        /// <param name="groupUserModel">L'objet contenant les informations du contributeur et de la base de données</param>
        /// <returns>Retourne le code statut HTTP Ok si l'ajout a été fait, BadRequest ou Conflict sinon
        /// </returns>
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

            DatabaseGroupUser databaseGroupUser = service.AddContributor(this.GetJWTIdentity().Name, groupUserModel);
            if (databaseGroupUser == null)
            {
                return Conflict();
            }

            //return CreatedAtRoute("DefaultApi", new { id = databaseGroupUser. }, databaseGroupUser);
            return Ok(databaseGroupUser);
        }

        // PUT: api/Contributor/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Contributor/5
        public void Delete(int id)
        {
        }
    }
}
