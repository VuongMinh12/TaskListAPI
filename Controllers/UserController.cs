using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static TaskListAPI.Model.Login;
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

        [Route("Login")]
        [HttpPost]
        public LoginResponse LogAcc(LoginRequest request)
        {
            try
            {
                //return new LoginResponse { UserName= request.username };
                return userRespository.Login(request);
            }
            catch (Exception ex) { return new LoginResponse{ message=ex.Message}; }
        }

        [Route("Signup")]
        [HttpPost]
        public Task<BaseResponse> SignUp(SignUpRequest request)
        {
            var signup = userRespository.SignUp(request);
            return signup;
        }

        [Route("ForgotPassword")]
        [HttpPost]
        public Task<BaseResponse> ForgotPass (ForgotPass requets)
        {
            var forgot = userRespository.ForgotPass(requets);
            return forgot;
        }
    }
}
