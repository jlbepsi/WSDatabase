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
    /*
     * TODO: Rétablir la sécurité
     * 
     * [JWTAuthenticationFilter("ROLE_SUPER_ADMIN")]
     * 
     */
    public class ServerNamesController : ApiController
    {
        private ServerNameService service = new ServerNameService();

        // GET: api/DatabaseServerNames
        public IList<DatabaseServerName> GetDatabaseServerNames()
        {
            return service.Get();
        }

        // GET: api/DatabaseServerNames/5
        [ResponseType(typeof(DatabaseServerName))]
        public IHttpActionResult GetDatabaseServerName(int id)
        {
            DatabaseServerName databaseServerName = service.Get(id);
            if (databaseServerName == null)
            {
                return NotFound();
            }

            return Ok(databaseServerName);
        }

        // PUT: api/DatabaseServerNames/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDatabaseServerName(int id, DatabaseServerName databaseServerName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != databaseServerName.Id)
            {
                return BadRequest();
            }

            if (!service.Update(databaseServerName))
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/DatabaseServerNames
        [ResponseType(typeof(DatabaseServerName))]
        public IHttpActionResult PostDatabaseServerName(DatabaseServerName databaseServerName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!service.Add(databaseServerName))
            {
                return Conflict();
            }

            return CreatedAtRoute("DefaultApi", new { id = databaseServerName.Id }, databaseServerName);
        }

        // DELETE: api/DatabaseServerNames/5
        [ResponseType(typeof(DatabaseServerName))]
        public IHttpActionResult DeleteDatabaseServerName(int id)
        {
            DatabaseServerName databaseServerName = service.Remove(id);
            if (databaseServerName == null)
            {
                return NotFound();
            }

            return Ok(databaseServerName);
        }

        protected override void Dispose(bool disposing)
        {
            service.Dispose(disposing);
        }
    }
}