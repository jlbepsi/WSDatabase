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
    public class ServerDatabaseController : ApiController
    {
        private DatabaseService service = new DatabaseService();

        // GET: api/DatabaseDBs
        public List<DatabaseDB> GetDatabaseDBs()
        {
            return service.GetDatabases();
        }

        // GET: api/DatabaseDBs/5
        [ResponseType(typeof(DatabaseDB))]
        public IHttpActionResult GetDatabaseDBs(int id)
        {
            DatabaseDB databaseDB = service.GetDatabase(id);
            if (databaseDB == null)
            {
                return NotFound();
            }

            return Ok(databaseDB);
        }

        // PUT: api/DatabaseDBs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDatabaseDB(int id, DatabaseDB databaseDB)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != databaseDB.ServerId)
            {
                return BadRequest();
            }

            if (!service.UpdateDatabase(databaseDB))
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/DatabaseDBs
        [ResponseType(typeof(DatabaseDB))]
        public IHttpActionResult PostDatabaseDB(DatabaseDB databaseDB)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!service.AddDatabase(databaseDB))
            {
                return Conflict();
            }

            return CreatedAtRoute("DefaultApi", new { id = databaseDB.Id }, databaseDB);
        }

        // DELETE: api/DatabaseDBs/5
        [ResponseType(typeof(DatabaseDB))]
        public IHttpActionResult DeleteDatabaseDB(int id)
        {
            DatabaseDB databaseDB = service.RemoveDatabase(id);
            if (databaseDB == null)
            {
                return NotFound();
            }

            return Ok(databaseDB);
        }

        protected override void Dispose(bool disposing)
        {
            service.Dispose(disposing);
        }
    }
}