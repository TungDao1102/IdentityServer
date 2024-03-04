using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            //     return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
            return new JsonResult(User.Claims.Select(x => new { x.Type, x.Value }));
        }
    }
}
