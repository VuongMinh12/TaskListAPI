namespace TaskListAPI.Model
{
    public class StatusResponse
    {
        public int No { get; set; }
        public int StatusId {get; set; }
        public string StatusName {get; set; }
        public bool IsActive {get; set; }
    }
    public class StatusRequest 
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? StatusName { get; set; }
        public int? IsActive { get; set; }

    }

    public class RequestStatusAddUp : BaseRequest
    {
        public RequestStatusObject status { get; set; }
    }
    public class RequestStatusObject
    {
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public int? IsActive { get; set; }
    }
    public class StatusDelete : BaseRequest
    {
        public int id { get; set; }
    }
}
