using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static TaskListAPI.Model.Login;
using TaskListAPI.Interface;

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
  }
}
