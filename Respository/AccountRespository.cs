using Dapper;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskListAPI.Interface;
using TaskListAPI.Model;
using static TaskListAPI.Model.Account;

namespace TaskListAPI.Respository
{
    public class AccountRespository : IAccountRespository
    {
        private readonly DapperContext _context;
        //minute
        private readonly int AccessTokenLifeSpan = 1;
        //minute
        private readonly int RefreshTokenLifeSpan = 30;
        public IConfiguration Configuration { get; }
        public AccountRespository(DapperContext context, IConfiguration configuration)
        {
            Configuration = configuration;
            _context = context;
        }
        public AccountResponse Login(AccountRequest request)
        {
            try
            {
                using (var con = _context.CreateConnection())
                {
                    var param = new DynamicParameters();
                    param.Add("@Email", request.email);
                    param.Add("@Password", GetSHA1HashData(request.password));

                    var logacc = con.Query<AccountObject>("LogAcc", param, commandType: CommandType.StoredProcedure).FirstOrDefault();

                    if (logacc == null)
                    {
                        return new AccountResponse
                        {
                            message = "Đăng nhập thất bại",
                            status = ResponseStatus.Fail,
                        };
                    }

                    string accessToken = CreateJwtAccessToken(logacc);
                    string refreshToken = CreateJwtRefreshToken();

                    var TokenParam = new DynamicParameters();
                    TokenParam.Add("@UserId", logacc.UserId);
                    TokenParam.Add("@RefreshToken", refreshToken);
                    TokenParam.Add("@RefreshTokenTime", DateTime.Now.AddMinutes(RefreshTokenLifeSpan));
                    con.Query("AddToken", TokenParam, commandType: CommandType.StoredProcedure);

                    return new AccountResponse
                    {
                        message = "Đăng nhập thành công",
                        status = ResponseStatus.Success,
                        Token = new TokenResponse
                        {
                            AccessToken = accessToken,
                            RefreshToken = refreshToken
                        },
                        Email = logacc.Email,
                        UserId = logacc.UserId,
                        RoleId = logacc.RoleId,
                    };
                }
            }
            catch (Exception ex) { return new AccountResponse { message = ex.Message }; }
        }

        public async Task<BaseResponse> SignUp(SignUpRequest request)
        {
            try
            {
                using (var con = _context.CreateConnection())
                {
                    var param = new DynamicParameters();

                    param.Add("@Email", request.Email);

                    string errorMess = "";
                    var logacc = con.Query<AccountObject>("Check_Signup", param, commandType: CommandType.StoredProcedure);

                    if (logacc.Any(u => u.Email == request.Email))
                    {
                         errorMess = "Email đã tồn tại!";
                    }
                    if (!String.IsNullOrEmpty(errorMess)) return new BaseResponse { message = errorMess, status = ResponseStatus.Fail };

                    else
                    {
                        var signup = new DynamicParameters();
                        signup.Add("@Email", request.Email);
                        signup.Add("@FirstName", request.FirstName);
                        signup.Add("@LastName", request.LastName);
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
            catch (Exception ex) { return new AccountResponse { message = ex.Message }; }
        }

        public async Task<BaseResponse> ForgotPass(ForgotPass request)
        {
            try
            {
                using (var con = _context.CreateConnection())
                {
                    var param = new DynamicParameters();
                    param.Add("@Email", request.Email);
                    param.Add("@FirstName", request.FirstName);
                    param.Add("@LastName", request.LastName);

                    var logacc = con.Query<AccountObject>("Check_ForgotPass", param, commandType: CommandType.StoredProcedure).FirstOrDefault();

                    if (logacc == null)
                    {
                        return new BaseResponse
                        {
                            status = ResponseStatus.Fail,
                            message = "Tài khoản không tồn tại"
                        };
                    }

                    var forgot = new DynamicParameters();
                    forgot.Add("@Email", logacc.Email);
                    forgot.Add("@FirstName", logacc.FirstName);
                    forgot.Add("@LastName", logacc.LastName);
                    forgot.Add("@Password", GetSHA1HashData(request.Password));
                    forgot.Add("@RoleId", logacc.RoleId);
                    forgot.Add("@UserId", logacc.UserId);

                    int rowsAffected = await con.ExecuteAsync("UpdateUser", forgot, commandType: CommandType.StoredProcedure);

                    if (rowsAffected > 0)
                    {
                        //HistoryRespository.RecordLog(request.currUserId, request.currUserName, (int)LogHIstory.UpdateTask, 0, true, _context);
                        return new BaseResponse
                        {
                            status = ResponseStatus.Success,
                            message = "Cập nhật password thành công"
                        };
                    }

                    return new BaseResponse
                    {
                        status = ResponseStatus.Fail,
                        message = "Không thể cập nhật password"
                    };
                }
            }
            catch (Exception ex) { return new AccountResponse { message = ex.Message }; }
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

        private string CreateJwtAccessToken(AccountObject AccountResponse)
        {
            var jwtSettings = Configuration.GetSection("JwtSettings");
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, AccountResponse.Email),
                new Claim(ClaimTypes.Name,AccountResponse.FirstName),
                new Claim(ClaimTypes.Name,AccountResponse.LastName),
                new Claim(ClaimTypes.Role,AccountResponse.RoleId.ToString())
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddMinutes(AccessTokenLifeSpan),
                SigningCredentials = credentials,
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

        private string CreateJwtRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(tokenBytes);
        }

        public BaseResponse RefreshToken(RefreshTokenRequest request)
        {
            try
            {
                using (var con = _context.CreateConnection())
                {
                    var param = new DynamicParameters();
                    param.Add("@UserId", request.UserId);
                    param.Add("@RefreshToken", request.RefreshToken);

                    var user = con.Query<AccountObject>("CheckAccountRefreshToken", param, commandType: CommandType.StoredProcedure).FirstOrDefault();

                    if (user != null)
                    {
                        string accessToken = CreateJwtAccessToken(user);
                        return new RefreshTokenResponse
                        {
                            message = "Refresh token thanh cong",
                            status = ResponseStatus.Success,
                            newAccessToken = accessToken
                        };
                    }
                    else
                    {
                        return new RefreshTokenResponse
                        {
                            message = "Refresh token that bai",
                            status = ResponseStatus.Fail,
                            newAccessToken = ""
                        };
                    }
                }
            }
            catch (Exception ex) { return new BaseResponse { message = ex.Message }; }
        }
    }
}

