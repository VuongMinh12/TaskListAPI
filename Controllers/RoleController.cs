using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskListAPI.Interface;
using TaskListAPI.Model;
using TaskListAPI.Respository;

namespace TaskListAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRespository roleRespository;
        public RoleController(IRoleRespository roleRespository)
        {
            this.roleRespository = roleRespository;
        }

        [HttpGet]
        public async Task<IEnumerable<RoleRespone>> GetRole([FromQuery] RoleRequest request)
        {
            try
            {
                var roll = await roleRespository.GetRole(request);
                return roll;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        [HttpPost]
        public async Task<BaseResponse> AddRole(RequestRoleAddUp request)
        {
            try
            {
                var addrole = await roleRespository.AddRole(request);
                return addrole;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        [Route("UpdateRole")]
        [HttpPut]
        public async Task<BaseResponse> UpdateRole(RequestRoleAddUp request)
        {
            try
            {
                var updaterole = await roleRespository.UpdateRole(request);
                return updaterole;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        [Route("DeleteRole")]
        [HttpPut]
        public async Task<BaseResponse> DeleteRole(RoleDelete delete)
        {
            try
            {
                var deleterole = await roleRespository.DeleteRole(delete);
                return deleterole;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
