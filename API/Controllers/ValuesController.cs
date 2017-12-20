using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace ch.wuerth.tobias.mux.API.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<String> Get()
        {
            return new[] {"value1", "value2"};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public String Get(Int32 id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] String value) { }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(Int32 id, [FromBody] String value) { }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(Int32 id) { }
    }
}