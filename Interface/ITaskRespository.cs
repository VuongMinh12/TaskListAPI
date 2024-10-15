using TaskListAPI.Model;

namespace TaskListAPI.Interface
{
    public interface ITaskRespository
    {
        public Task<BaseResponse> AddTask(TaskRequest task);
        public Task<BaseResponse> UpdateAssignee(UpdateAssignee assignee);
        public Task<IEnumerable<TaskResponse>> GetTask(GetTaskRequest request);
        public Task<BaseResponse> DeleteTask(int id);
    }
}
