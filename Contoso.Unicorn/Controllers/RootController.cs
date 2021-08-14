using Microsoft.AspNetCore.Mvc;
using Contoso.Unicorn.Responses;

namespace Contoso.Unicorn.Controllers
{
    [Route("/info")]
    [Route("/")]
    [ApiController]
    [Consumes("application/json")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RootController : ControllerBase
    {
        [HttpGet]
#pragma warning disable CA1822 // Mark members as static
        public IndexResponse Default()
#pragma warning restore CA1822 // Mark members as static
        {
            return new IndexResponse { };
        }
    }
}