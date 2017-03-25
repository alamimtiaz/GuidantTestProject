using MemoryCache;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using WebApi.Model.V1;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private Object thisLock = new Object();

        // GET api/values
        // Collections of Saved User
        // 204 No Content: when collection is empty
        // 200 OK
        [HttpGet]
        public IActionResult Get()
        {
            if (Cache.Exists<User>())
            {
                return new ObjectResult(Cache.GetMultiple<User>());
            }

            return NoContent();
        }

        // GET api/values/5
        // Returns User details with matching ID
        // 201 OK: when user with ID found
        // 404 Not Found: When user doesn’t exist.
        [HttpGet("{id}")]
        public IActionResult Get(HttpRequestMessage request, int id)
        {
            if(Cache.Exists(id.ToString()))
            {
                return new ObjectResult((User)Cache.Get(id.ToString()));
            }
            return NotFound();
        }

        // POST api/values
        // ID of the newly created customer
        // 201 Created: when successful entry is made
        // 400 Bad Request: When user already exists with message e.g.User Garrett already exists

       [HttpPost]
        public IActionResult Post([FromBody]User value)
        {
            if (value == null)
            {
                return BadRequest();
            }

           return AddNewUser(value);
        }


        // PATCH api/values/5/7234
        // Returns User details with matching ID
        // 200 OK: when user with ID found, and point updated
        // 404 Not Found: When user doesn’t exists.
        [HttpPatch("{id}/{points}")]
        public IActionResult Patch(int id, int points)
        {
            if (Cache.Exists(id.ToString()))
            {
                var user = new ObjectResult((User)Cache.Get(id.ToString()));
                ((User)user.Value).Points = points;
                Cache.Update(id.ToString(), user.Value);
                return Ok();
            }
            return NotFound();
        }


        private IActionResult AddNewUser(User value)
        {
            if (Cache.Exists(value.Name))
            {
                return BadRequest(String.Format("User {0} already exists", value.Name));
            }

            return AddToRepo(value);
        }

        private IActionResult AddToRepo(User Value)
        {
            lock (thisLock)
            {
                int newId = GetNewId();

                if (newId == 1)
                {
                    Cache.Store("lastKey", newId);
                }
                else
                {
                    Cache.Update("lastKey", newId);
                }
       
                Cache.Store(newId.ToString(), new User() { Name = Value.Name, Points = Value.Points });
                Cache.Store(Value.Name, newId);

                return Created("", newId);
            }
        }

        private int GetNewId()
        {
            return Cache.Exists("lastKey") ? (int)Cache.Get("lastKey") + 1 : 1;
        }
    }
}
