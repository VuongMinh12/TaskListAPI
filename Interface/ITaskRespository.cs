using TaskListAPI.Model;
using static TaskListAPI.Model.TaskAddUpdateRequest;

namespace TaskListAPI.Interface
{
    public interface ITaskRespository
    {
        public Task<BaseResponse> AddTask(TaskAddUpdateRequest task);
        public Task<BaseResponse> UpdateTask(TaskAddUpdateRequest assignee);
        public Task<IEnumerable<TaskResponse>> GetTask(GetTaskRequest request);
        public Task<BaseResponse> DeleteTask(TaskDelete delete);
    }
}
