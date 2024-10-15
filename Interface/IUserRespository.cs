using TaskListAPI.Model;

namespace TaskListAPI.Interface
{
    public interface IUserRespository
    {
        public UserListReponse AllUser();
        public List<TaskForUser> GetTaskAssignList();
    }
}
