using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataInterface;
using DataSetRest.Services;

namespace DataSetRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private readonly IDataSource _DataSource;
        // GET api/values

        public ValuesController(IDataSource DS)
        {
            _DataSource = DS;
        }
        [HttpGet]
        public  async Task<ActionResult<KeyValuePair<HKey, HDataObject>[]>> Get()
        {
            return await _DataSource.GetObjects().ToAsyncEnumerable().ToArray();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
