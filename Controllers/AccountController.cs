using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static TaskListAPI.Model.Account;
using TaskListAPI.Interface;
using TaskListAPI.Model;
using TaskListAPI.Respository;

namespace TaskListAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRespository accountRespository;
        public AccountController(IAccountRespository accountRespository)
        {
            this.accountRespository = accountRespository;
        }

        [Route("Login")]
        [HttpPost]
        public AccountResponse LogAcc(AccountRequest request)
        {
            try
            {
                return accountRespository.Login(request);
            }
            catch (Exception ex) { return new AccountResponse { message = ex.Message }; }
        }

        [Route("Signup")]
        [HttpPost]
        public Task<BaseResponse> SignUp(SignUpRequest request)
        {
            var signup = accountRespository.SignUp(request);
            return signup;
        }

        [Route("ForgotPassword")]
        [HttpPost]
        public Task<BaseResponse> ForgotPass(ForgotPass requets)
        {
            var forgot = accountRespository.ForgotPass(requets);
            return forgot;
        }

        [Route("Refresh")]
        [HttpPost]
        public BaseResponse RefreshToken(RefreshTokenRequest request)
        {
            return accountRespository.RefreshToken(request);
        }
    }
}
