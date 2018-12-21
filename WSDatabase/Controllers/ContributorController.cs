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

        // GET: api/Contributor
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Contributor/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Contributor
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
