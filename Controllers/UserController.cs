using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskListAPI.Interface;
using TaskListAPI.Model;
using TaskListAPI.Respository;

namespace TaskListAPI.Controllers
{
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
        public UserListReponse AllUser([FromQuery]BaseRequest request)
        {
            try
            {
                return  userRespository.AllUser();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        [Route("UserTask")]
        [HttpGet]
        public List<TaskForUser> GetTaskAssignList([FromQuery] BaseRequest request)
        {
            try
            {
                return userRespository.GetTaskAssignList();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
