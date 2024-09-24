using TaskListAPI.Model;

namespace TaskListAPI.Interface
{
  public interface IUserRespository
   {
     public Login.LoginResponse Login(Login.LoginRequest request);

   }
}

