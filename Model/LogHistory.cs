namespace TaskListAPI.Model
{
    public class LogHistory : BaseRequest
    {
        public int UserId { get; set; }
        public int LogType { get; set; }
        public DateTime LogTime { get; set; }
        public int? TargetTask { get; set; }
        public int? TargetUser { get; set; }
    }
}
