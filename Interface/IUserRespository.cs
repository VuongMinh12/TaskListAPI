using TaskListAPI.Model;

namespace TaskListAPI.Interface
{
    public interface IUserRespository
    {
        public Task<UserListReponse> AllUser(BaseRequest request);
        public Task<UserTaskList> GetTaskAssignList();
        public Task<BaseResponse> AddUser (UserRequest request);
        public Task<BaseResponse> UpdateUser (UserRequest request);
        public Task<BaseResponse> DeleteUser (UserDelete delete);
        public Task<IEnumerable<GetUserResponse>> GetAllUser(GetUserRequest request);
     }
}
