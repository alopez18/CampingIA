using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingAI.WebApi.Controllers.api {
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BaseAuthorizedController : ControllerBase {
    }
}
