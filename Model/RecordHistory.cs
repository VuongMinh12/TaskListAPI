namespace TaskListAPI.Model
{
    public class RecordHistory : BaseRequest
    {
        public string Content { get; set; }
        public LogHIstory LogType { get; set; }
        public DateTime LogTime { get; set; }
        public int UserId { get; set; }
        public int TargetTask { get; set; }
        public int TargetUser { get; set; }

    }
    public enum LogHIstory
    {
        AddTask = 1,
        UpdateTask = 2,
        DeleteTask = 3,
        ChangePw = 4
    }
}
