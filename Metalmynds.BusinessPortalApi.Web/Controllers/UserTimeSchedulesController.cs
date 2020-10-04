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
    public class UserTimeSchedulesController : ControllerBase
    {
        static BusinessPortalClient _client;

        public UserTimeSchedulesController(BusinessPortalClient client) 
        {
            _client = client;
        }

        // GET: api/<UserTimeSchedulesController>
        [HttpGet]
        public async Task<IEnumerable<UserTimeSchedule>> Get()
        {
            return await _client.GetUserTimeSchedules();
        }

        // GET api/<UserTimeSchedulesController>/5
        [HttpGet("{id}")]
        public string Get(String id)
        {
            return "value" + id;
        }

        // POST api/<UserTimeSchedulesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserTimeSchedulesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserTimeSchedulesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

}
