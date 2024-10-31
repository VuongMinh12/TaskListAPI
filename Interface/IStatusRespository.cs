using TaskListAPI.Model;

namespace TaskListAPI.Interface
{
    public interface IStatusRespository
    {
        public Task<IEnumerable<StatusResponse>> GetStatus(StatusRequest request);
        public Task<BaseResponse> AddStatus(RequestStatusAddUp request);
        public Task<BaseResponse> UpdateStatus(RequestStatusAddUp request);
        public Task<BaseResponse> DeleteStatus(StatusDelete delete);
    }
}
