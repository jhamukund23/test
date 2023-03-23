using Dss.API.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dss.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProducerController : ControllerBase
    {
        private readonly RequestCommandHandler _handler;
        public ProducerController(RequestCommandHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        ///     Publishing data on a specific topic
        /// </summary>
        /// <param name="request"> holds topic's name and data to publish</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ProduceData(RequestCommand request)
        {
            try
            {
                var result = await _handler.Handle(request);
                return await Task.FromResult(Ok());
            }
            catch (Exception)
            {
                return await Task.FromResult(StatusCode(500));
            }

        }
    }
}
