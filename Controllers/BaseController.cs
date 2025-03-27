using Microsoft.AspNetCore.Mvc;
using EtaxService.Installers;
using Microsoft.AspNetCore.Authorization;

namespace EtaxService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class BaseController : ControllerBase
    {
    }
}
