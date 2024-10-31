using TaskListAPI.Model;

namespace TaskListAPI.Interface
{
    public interface IRoleRespository
    {
        public Task<IEnumerable<RoleRespone>> GetRole(RoleRequest request);
        public Task<BaseResponse> AddRole(RequestRoleAddUp request);
        public Task<BaseResponse> UpdateRole(RequestRoleAddUp request);
        public Task<BaseResponse> DeleteRole(RoleDelete delete);
    }
}
