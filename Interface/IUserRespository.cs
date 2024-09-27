using TaskListAPI.Model;
using static TaskListAPI.Model.Login;

namespace TaskListAPI.Interface
{
  public interface IUserRespository
   {
     public LoginResponse Login(LoginRequest request);
     public Task<BaseResponse> SignUp(SignUpRequest request);
     public Task<BaseResponse> ForgotPass(ForgotPass request);
   }
}

