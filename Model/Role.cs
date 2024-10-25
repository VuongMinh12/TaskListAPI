namespace TaskListAPI.Model
{
    public class RoleRespone
    {
        public int No { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }
    public class RoleRequest : BaseRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? RoleName { get; set; }
        public int? IsActive { get; set; }
    }
    public class RequestRoleAddUp : BaseRequest
    {
        public RequestRoleObject role { get; set; }
    }
    public class RequestRoleObject
    {
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public int? IsActive { get; set; }
    }
    public class RoleDelete : BaseRequest
    {
        public int id { get; set; }
    }
}
