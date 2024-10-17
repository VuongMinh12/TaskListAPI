namespace TaskListAPI.Model
{
    public class UserResponse 
    {
        public int UserId { get; set; }
        public string Email { get; set; }
    }

    public class UserListReponse : BaseResponse
    {
        public List<UserResponse> users { get; set; }
    }
    public class TaskForUser {
        public int TaskId { get; set; }
        public int UserId { get; set; }
    }

    public class UserTaskList : BaseResponse
    {
        public List<TaskForUser> usersTask { get; set; }
    }
}
