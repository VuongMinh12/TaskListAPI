using TaskListAPI.Model;
using static TaskListAPI.Model.Account;

namespace TaskListAPI.Interface
{
    public interface IAccountRespository
    {
        public AccountResponse Login(AccountRequest request);
        public Task<BaseResponse> SignUp(SignUpRequest request);
        public Task<BaseResponse> ForgotPass(ForgotPass request);
        public BaseResponse RefreshToken(RefreshTokenRequest request);
    }
}
