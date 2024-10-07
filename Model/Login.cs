namespace TaskListAPI.Model
{
    public class Login
    {
        public class LoginRequest 
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        public class LoginObject{
            public int UserId { get; set; }
            public string Email { get; set; }
            public string UserName { get; set; }
            public int RoleId { get; set; }
        }

        public class LoginResponse : BaseResponse
        {
            public string Token { get; set; }
            public int UserId { get; set; }
            public string Email { get; set; }
            public string UserName { get; set; }
            public int RoleId { get; set; }
            public string RefreshToken { get; set; }

        }

        public class SignUpRequest : BaseRequest 
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
            public int RoleId { get; set; }
        }
        public class ForgotPass : BaseRequest
        {
            public string Email { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            
        }
    }
}
