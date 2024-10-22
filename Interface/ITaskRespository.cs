using TaskListAPI.Model;
using static TaskListAPI.Model.TaskAddUpdateRequest;

namespace TaskListAPI.Interface
{
    public interface ITaskRespository
    {
        public Task<BaseResponse> AddTask(TaskAddUpdateRequest request);
        public Task<BaseResponse> UpdateTask(TaskAddUpdateRequest request);
        public Task<IEnumerable<TaskResponse>> GetTask(GetTaskRequest request);
        public Task<BaseResponse> DeleteTask(TaskDelete delete);
    }
}
