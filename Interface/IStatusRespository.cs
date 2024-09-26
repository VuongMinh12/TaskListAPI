using TaskListAPI.Model;

namespace TaskListAPI.Interface
{
    public interface IStatusRespository
    {
        public Task<IEnumerable<StatusResponse>> GetStatus(StatusRequest status);
    }
}
