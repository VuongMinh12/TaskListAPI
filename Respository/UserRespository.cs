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

                    string token = CreateJwt(new LoginObject
                    {
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

        public async Task<BaseResponse> SignUp(SignUpRequest request)
        {
            try
            {               
                using (var con = _context.CreateConnection())
                {
                    var param = new DynamicParameters();
                    param.Add("@PageNumber", 1);
                    param.Add("@PageSize", 0);
                    param.Add("@UserName", "");
                    param.Add("@Email", "");
                    param.Add("@RoleId ", null);
                    param.Add("@IsActive ", 1);

                    string errorMess = "da ton tai";

                    var logacc = con.Query<LoginObject>("GetUser", param, commandType: CommandType.StoredProcedure);

                    if(logacc.Any(u => u.UserName == request.UserName))
                    {
                        errorMess = "Username " + errorMess;
                        return new BaseResponse { message = errorMess, status = ResponseStatus.Fail };
                    }
                    if (logacc.Any(u => u.Email == request.Email))
                    {
                        errorMess = "Email " + errorMess;
                        return new BaseResponse { message = errorMess, status = ResponseStatus.Fail };
                    }



                    else {
                        var signup = new DynamicParameters();
                        signup.Add("@Email", request.Email);
                        signup.Add("@UserName", request.UserName);
                        signup.Add("@Password", request.Password);
                        signup.Add("@RoleId ", request.RoleId);

                        var id = Convert.ToInt32(await con.ExecuteScalarAsync("AddUser", signup, commandType: CommandType.StoredProcedure));
                    if (id > 0)
                    {
                        return new BaseResponse
                        {
                            status = ResponseStatus.Success,
                            message = "Them thanh cong"
                        };
                    }

                    return new BaseResponse
                    {
                        status = ResponseStatus.Fail,
                        message = "Khong the tao tai khoan"
                    };
                    }
                }

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
