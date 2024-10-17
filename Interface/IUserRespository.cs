using TaskListAPI.Model;

namespace TaskListAPI.Interface
{
    public interface IUserRespository
    {
        public Task<UserListReponse> AllUser();
        public Task<UserTaskList> GetTaskAssignList();
    }
}
