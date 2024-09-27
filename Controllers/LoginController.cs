using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static TaskListAPI.Model.Login;
using TaskListAPI.Interface;
using TaskListAPI.Model;

namespace TaskListAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserRespository userRespository;
        public LoginController(IUserRespository userRespository)
        {
            this.userRespository = userRespository;
        }

        [Route("Login")]
        [HttpPost]
        public LoginResponse LogAcc(LoginRequest request)
        {
            try
            {
                var logacc = userRespository.Login(request);
                return logacc;

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
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
