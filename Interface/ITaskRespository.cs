using TaskListAPI.Model;

namespace TaskListAPI.Interface
{
    public interface ITaskRespository
    {
        public Task<IEnumerable<TaskResponse>> GetTask(TaskRequest request);
        public Task<int> AddTask(TaskResponse task);
        public Task<bool> UpdateTask(TaskResponse task);
        public Task<bool> DeleteTask(int id);

    }
}
