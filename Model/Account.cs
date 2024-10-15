namespace TaskListAPI.Model
{
    public class Account
    {
        public class AccountRequest 
        {
            public string email { get; set; }
            public string password { get; set; }
        }

        public class AccountObject{
            public int UserId { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int RoleId { get; set; }

        }

        public class AccountResponse : BaseResponse
        {
            public TokenResponse Token { get; set; }
            public int UserId { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int RoleId { get; set; }

        }

        public class SignUpRequest 
        {
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Password { get; set; }
            public int RoleId { get; set; }
        }

        public class ForgotPass : BaseRequest
        {
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Password { get; set; }
            
        }
    }
}