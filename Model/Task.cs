namespace TaskListAPI.Model
{
    public class TaskAddUpdateRequest : BaseRequest
    {
        public TaskAddUpRequestObject task { get; set; }
    }
    public class TaskAddUpRequestObject
    {
        public int? TaskId { get; set; }
        public string Title { get; set; }
        public int StatusId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime FinishDate { get; set; }
        public int Estimate { get; set; }
        public List<int>? ListUser { get; set; }
    }

    public class GetTaskRequest : BaseRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Title { get; set; }
        public int? StatusId { get; set; }
        public string? CreateDate { get; set; }
        public string? FinishDate { get; set; }

    }

    public class TaskResponse : BaseResponse
    {
        public int No { get; set; }
        public int TaskId { get; set; }
        public string Title { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime FinishDate { get; set; }
        public int Estimate { get; set; }
    }

    public class TaskDelete : BaseRequest
    {
        public int id { get; set; }
    }
}
