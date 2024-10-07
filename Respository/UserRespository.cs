using Dapper;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
                    param.Add("@Password", GetSHA1HashData(request.password));

                    var logacc = con.Query<LoginObject>("LogAcc", param, commandType: CommandType.StoredProcedure);

                    if (logacc.Count() == 0 || logacc == null)
                    {
                        return new LoginResponse
                        {
                            message = "Đăng nhập thất bại",
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
                        message = "Đăng nhập thành công",
                        status = ResponseStatus.Success,
                        Token = token,
                        UserName = logacc.FirstOrDefault().UserName,
                        Email = logacc.FirstOrDefault().Email,
                        UserId = logacc.FirstOrDefault().UserId,
                        RoleId = logacc.FirstOrDefault().RoleId,
                    };
                }
            }
            catch (Exception ex) { return new LoginResponse { message = ex.Message }; }
        }


        public async Task<BaseResponse> SignUp(SignUpRequest request)
        {
            try
            {
                using (var con = _context.CreateConnection())
                {
                    var param = new DynamicParameters();

                    param.Add("@UserName", request.UserName);
                    param.Add("@Email", request.Email);

                    string errorMess = "";
                    var logacc = con.Query<LoginObject>("Check_Signup", param, commandType: CommandType.StoredProcedure);

                    if (logacc.Any(u => u.UserName == request.UserName))
                    {
                        errorMess += "Username đã tồn tại! ";
                    }
                    if (logacc.Any(u => u.Email == request.Email))
                    {
                        errorMess += " Email đã tồn tại!";
                    }
                    if (!String.IsNullOrEmpty(errorMess)) return new BaseResponse { message = errorMess, status = ResponseStatus.Fail };

                    else
                    {
                        var signup = new DynamicParameters();
                        signup.Add("@Email", request.Email);
                        signup.Add("@UserName", request.UserName);
                        signup.Add("@Password", GetSHA1HashData(request.Password));
                        signup.Add("@RoleId ", request.RoleId);

                        var id = Convert.ToInt32(await con.ExecuteScalarAsync("AddUser", signup, commandType: CommandType.StoredProcedure));
                        if (id > 0)
                        {
                            return new BaseResponse
                            {
                                status = ResponseStatus.Success,
                                message = "Đăng ký tài khoản thành công"
                            };
                        }

                        return new BaseResponse
                        {
                            status = ResponseStatus.Fail,
                            message = "Không thể đăng ký tài khoản"
                        };
                    }
                }
            }
            catch (Exception ex) { return new LoginResponse { message = ex.Message }; }
        }

        public async Task<BaseResponse> ForgotPass(ForgotPass request)
        {
            try
            {
                using (var con = _context.CreateConnection())
                {
                    var param = new DynamicParameters();
                    param.Add("@UserName", request.UserName);
                    param.Add("@Email", request.Email);

                    var logacc = con.Query<LoginObject>("Check_ForgotPass", param, commandType: CommandType.StoredProcedure).FirstOrDefault();

                    if (logacc == null)
                    {
                        return new BaseResponse
                        {
                            status = ResponseStatus.Fail,
                            message = "User khong ton tai"
                        };
                    }

                    var forgot = new DynamicParameters();
                    forgot.Add("@Email", logacc.Email);
                    forgot.Add("@UserName", logacc.UserName);
                    forgot.Add("@Password", GetSHA1HashData(request.Password));
                    forgot.Add("@RoleId", logacc.RoleId);
                    forgot.Add("@UserId", logacc.UserId);

                    int rowsAffected = await con.ExecuteAsync("UpdateUser", forgot, commandType: CommandType.StoredProcedure);

                    HistoryRespository.RecordLog(request.currUserId, request.currUserName, (int)LogHIstory.UpdateTask, 0 ,true, _context);
                    if (rowsAffected > 0)
                    {
                        return new BaseResponse
                        {
                            status = ResponseStatus.Success,
                            message = "Cap nhat password thanh cong"
                        };
                    }

                    return new BaseResponse
                    {
                        status = ResponseStatus.Fail,
                        message = "Cap nhat password khong thanh cong"
                    };
                }
            }
            catch (Exception ex) { return new LoginResponse { message = ex.Message }; }
        }

        private string GetSHA1HashData(string data)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(data));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
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

        private string CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);

            var tokenInUser = _authContext.Users
                .Any(a => a.RefreshToken == refreshToken);
            if (tokenInUser)
            {
                return CreateRefreshToken();
            }
            return refreshToken;
        }

        private ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes("stringgggggggggggggggggggggggsecrettokennnnnnnnn");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("This is Invalid Token");
            return principal;

        }
    }
}
