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
}
