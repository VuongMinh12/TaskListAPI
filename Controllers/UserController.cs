using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskListAPI.Interface;
using TaskListAPI.Model;
using TaskListAPI.Respository;

namespace TaskListAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRespository userRespository;
        public UserController(IUserRespository userRespository)
        {
            this.userRespository = userRespository;
        }
        [Route("AllUser")]
        [HttpGet]
        public async Task<UserListReponse> AllUser([FromQuery]BaseRequest request)
        {
            try
            {
                return await userRespository.AllUser(request);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        [Route("UserTask")]
        [HttpGet]
        public async Task<UserTaskList> GetTaskAssignList([FromQuery] BaseRequest request)
        {
            try
            {
                return await userRespository.GetTaskAssignList();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        [HttpPost]
        public async Task<BaseResponse> AddUser(UserRequest request)
        {
            try
            {
                return await userRespository.AddUser(request);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        [Route("UpdateUser")]
        [HttpPut]
        public async Task<BaseResponse> UpdateUser(UserRequest request)
        {
            try
            {
                return await userRespository.UpdateUser(request);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        [Route("DeleteUser")]
        [HttpPut]
        public async Task<BaseResponse> DeleteeUser(UserDelete delete)
        {
            try
            {
                return await userRespository.DeleteUser(delete);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        [Route("GetAllUser")]
        [HttpGet]
        public  async Task<IEnumerable<GetUserResponse>> GetAllUser([FromQuery] GetUserRequest request)
        {
            try
            {
                var getUser = await userRespository.GetAllUser(request);
                return getUser;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
