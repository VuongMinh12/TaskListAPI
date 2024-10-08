namespace TaskListAPI.Model
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class RefreshTokenResponse : BaseResponse
    {
        public string newAccessToken { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
        public int UserId { get; set; }
    }
}
