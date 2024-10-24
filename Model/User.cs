using System.ComponentModel.DataAnnotations;

namespace TaskListAPI.Model
{
    public class UserResponse 
    {
        public int UserId { get; set; }
        public string Email { get; set; }
    }

    public class UserListReponse : BaseResponse
    {
        public List<UserResponse> users { get; set; }
    }
    public class TaskForUser {
        public int TaskId { get; set; }
        public int UserId { get; set; }
    }

    public class UserTaskList : BaseResponse
    {
        public List<TaskForUser> usersTask { get; set; }
    }

    public class UserRequest : BaseRequest
    {
        public UserRequestObject user { get; set; }
    }

    public class UserRequestObject
    { 
        public int? UserId { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? RoleId { get; set; }
        public string? Password { get; set; }
    }
    public class UserDelete : BaseRequest
    {
        public int id { get; set; }
    }
    public class GetUserRequest : BaseRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? RoleId { get; set; }
        public int? IsActive { get; set; }
    }
    public class GetUserResponse : BaseResponse 
    { 
        public int No { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }
}
