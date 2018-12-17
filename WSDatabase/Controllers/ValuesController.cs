using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace WSDatabase.Controllers
{
    public class Value
    {
        public int Id { get; set; }
        public string Libelle { get; set; }
    }

    public class ValuesController : ApiController
    {
        private List<Value> values = new List<Value>(10);

        private Value Find(int id)
        {
            foreach(Value avalue in values)
            {
                if (avalue.Id == id)
                    return avalue;
            }
            return null;
        }

        public ValuesController()
        {
            for(int i=1; i<11; i++)
            {
                values.Add(new Value() { Id = i, Libelle = string.Format("value-{0}", i) });
            }
        }

        // GET api/values
        public IEnumerable<Value> Get()
        {
            return values;
        }

        // GET api/values/5
        public IHttpActionResult Get(int id)
        {
            Value existingValue = this.Find(id);
            if (existingValue == null)
            {
                return NotFound();
            }

            return Ok(existingValue);
        }

        // POST api/values
        [ResponseType(typeof(Value))]
        public IHttpActionResult Post(Value aValue)
        {
            if (aValue == null ||string.IsNullOrWhiteSpace(aValue.Libelle))
            {
                return BadRequest(ModelState);
            }

            //System.Security.Principal.IPrincipal principal = this.RequestContext.Principal;

            values.Add(aValue);
            return CreatedAtRoute("DefaultApi", new { id = aValue.Id }, aValue);
        }

        // PUT api/values/5
        public IHttpActionResult Put(int id, Value aValue)
        {
            Value existingValue = this.Find(id);
            if (existingValue != null)
            {
                existingValue.Libelle = aValue.Libelle;
                return StatusCode(HttpStatusCode.NoContent);
            }

            return NotFound();
        }

        // DELETE api/values/5
        public IHttpActionResult Delete(int id)
        {
            Value existingValue = this.Find(id);
            if (existingValue == null)
            {
                return NotFound();
            }

            values.Remove(existingValue);
            return Ok(existingValue);
        }
    }
}
