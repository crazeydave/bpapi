using Metalmynds.BusinessPortalApi.Client;
using Metalmynds.BusinessPortalApi.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Metalmynds.BusinessPortalApi.Web
{

    [Route("api/[controller]")]
    [ApiController]
    public class SelectiveCallRulesController : ControllerBase
    {
        static BusinessPortalClient _client;

        public SelectiveCallRulesController(BusinessPortalClient client) 
        {
            _client = client;
        }

        // GET: api/<SelectiveCallRulesController>
        [HttpGet]
        public async Task<IEnumerable<SelectiveCallRule>> Get()
        {
            return await _client.GetSelectiveCallRules();
        }

        // GET api/<SelectiveCallRulesController>/5
        [HttpGet("{id}")]
        public async Task<SelectiveCallRule> Get(String id)
        {
            return await _client.GetSelectiveCallRule(id);
        }

        // POST api/<SelectiveCallRulesController>
        [HttpPost]
        public async Task Post([FromBody] SelectiveCallRule rule)        
        {               
            await _client.CreateSelectiveCallRule(rule);        
        }

        // PUT api/<SelectiveCallRulesController>/AA Saturday 10:11 - 22:12
        [HttpPut("{id}")]
        public async Task Put(String id, [FromBody] SelectiveCallRule value)
        {
            await _client.UpdateSelectiveCallRule(id, value);
        }

        // DELETE api/<SelectiveCallRulesController>/5
        [HttpDelete("{id}")]
        public void Delete(String id)
        {
        }
    }
}
