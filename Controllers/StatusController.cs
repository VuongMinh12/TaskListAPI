using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskListAPI.Interface;
using TaskListAPI.Model;

namespace TaskListAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IStatusRespository statusRespository;
        public StatusController(IStatusRespository statusRespository)
        {
            this.statusRespository = statusRespository;
        }

        [HttpGet]
        public async Task<IEnumerable<StatusResponse>> GetStatus ([FromQuery] StatusRequest status)
        {
            var getStatus = await statusRespository.GetStatus(status);
            return getStatus;
        }
    }
}
