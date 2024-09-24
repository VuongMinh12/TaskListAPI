namespace TaskListAPI.Model
{
    public class TaskResponse : BaseResponse
    {
        public int No { get; set; }
        public int TaskId { get; set; }
        public string? Title { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime FinishDate { get; set; }
        public int Estimate { get; set; }
        public int UserId { get; set; }
    }

    public class TaskRequest : BaseRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Title { get; set; }
        public int? StatusId { get; set; }
        public string? CreateDate { get; set; }
        public string? FinishDate { get; set; }
        public int? UserId { get; set; }
    }

}
