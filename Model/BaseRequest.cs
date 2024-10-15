namespace TaskListAPI.Model
{
    public class BaseRequest
    {
        public int currUserId { get; set; }
        public string currEmail { get; set; }
        public int UserRole { get; set; }
    }
    public class BaseResponse
    {
        public ResponseStatus status { get; set; }
        public string message { get; set; }
        
    }

    public enum ResponseStatus {
        Success = 1,
        Fail = 2,
        NotAllow = 3,
        NotFound = 4
    };
}
