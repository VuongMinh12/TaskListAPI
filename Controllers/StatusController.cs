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
            try
            {
                var getStatus = await statusRespository.GetStatus(status);
                return getStatus;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        [HttpPost]
        public async Task<BaseResponse> AddStatus(RequestStatusAddUp request)
        {
            try
            {
                var addStatus = await statusRespository.AddStatus(request);
                return addStatus;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        [Route("UpdateStatus")]
        [HttpPut]
        public async Task<BaseResponse> UpdateStatus(RequestStatusAddUp request)
        {
            try
            {
                var updateStatus = await statusRespository.UpdateStatus(request);
                return updateStatus;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        [Route("DeleteStatus")]
        [HttpPut]    
        public async Task<BaseResponse> DeleteStatus(StatusDelete delete)
        {
            try
            {
                var deleteStatus = await statusRespository.DeleteStatus(delete);
                return deleteStatus;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
