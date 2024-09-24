using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskListAPI.Interface;
using TaskListAPI.Model;
using static TaskListAPI.Model.Login;


namespace TaskListAPI.Respository
{
    public class UserRespository : IUserRespository
    {
        private readonly DapperContext _context;
        public UserRespository(DapperContext context)
        {
            _context = context;
        }

        public LoginResponse Login(LoginRequest request)
        {
            try
            {
                using (var con = _context.CreateConnection())
                {
                    var param = new DynamicParameters();
                    param.Add("@UserName", request.username);
                    param.Add("@Password", request.password);

                    var logacc = con.Query<LoginObject>("LogAcc", param, commandType: CommandType.StoredProcedure);

                    if (logacc.Count() == 0 || logacc == null)
                    {
                        return new LoginResponse
                        {
                            message = "Login khong thanh cong",
                            status = ResponseStatus.Fail,
                        };
                    }

                    string token = CreateJwt(new LoginObject {
                        UserName = logacc.FirstOrDefault().UserName,
                        Email = logacc.FirstOrDefault().Email,
                        RoleId = logacc.FirstOrDefault().RoleId
                    });

                    return new LoginResponse
                    {
                        message = "Login thanh cong",
                        status = ResponseStatus.Success,
                        Token = token,
                        UserName = logacc.FirstOrDefault().UserName,
                        Email = logacc.FirstOrDefault().Email,
                        UserId = logacc.FirstOrDefault().UserId,
                        RoleId = logacc.FirstOrDefault().RoleId,
                    };
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        private string CreateJwt(LoginObject LoginResponse)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("stringgggggggggggggggggggggggsecrettokennnnnnnnn");


            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, LoginResponse.Email),
                new Claim(ClaimTypes.Name,LoginResponse.UserName),
                new Claim(ClaimTypes.Role,LoginResponse.RoleId.ToString())
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
