using TaskListAPI.Model;

namespace TaskListAPI.Interface
{
    public interface ITaskRespository
    {
        public Task<IEnumerable<TaskResponse>> GetTask(TaskRequest request);
        public Task<BaseResponse> AddTask(TaskAddUpRequest task);
        public Task<BaseResponse> UpdateTask(TaskAddUpRequest task);
        public Task<BaseResponse> DeleteTask(TaskDelete task);

    }
}
