using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _ctx;
        public ValuesController(DataContext ctx)
        {
            this._ctx = ctx;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Value>>> Get()
        {
            List<Value> result = new List<Value>();
            result = await _ctx.Values.ToListAsync();
            return result;
        }

        // GET api/values/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _ctx.Values.FirstOrDefaultAsync(e => e.Id == id));

        // POST api/values
        [HttpPost]
        public void Post([FromBody] Value value)
        {
            _ctx.Add(value);
            _ctx.SaveChanges();
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
            Value val = _ctx.Values.FirstOrDefault(e => e.Id == id);
            if (val != null){
                _ctx.Values.Remove(val);
                _ctx.SaveChanges();
            }           
        }
    }
}
