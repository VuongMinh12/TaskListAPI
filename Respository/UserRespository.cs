using Dapper;
using TaskListAPI.Interface;
using TaskListAPI.Model;
using System.Data;
using static TaskListAPI.Model.Account;
using Azure.Core;

namespace TaskListAPI.Respository
{
    public class UserRespository : IUserRespository
    {
        private readonly DapperContext context;
        public UserRespository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<BaseResponse> AddUser(UserRequest request)
        {
            try
            {
                using (var con = context.CreateConnection())
                {
                    var check = new DynamicParameters();
                    check.Add("@Email", request.user.Email);
                    check.Add("@UserId", request.user.UserId);
                    var checkEmail = await con.QueryAsync("CheckUpdateAddUser", check, commandType: CommandType.StoredProcedure);
                    if (checkEmail.Count() > 0)
                    {
                        return new BaseResponse { status = ResponseStatus.Fail, message = "Email này đã tồn tại. Vui lòng dùng email khác" };
                    }
                    else
                    {
                        var add = new DynamicParameters();
                        add.Add("@Email", request.user.Email);
                        add.Add("@FirstName", request.user.FirstName);
                        add.Add("@LastName", request.user.LastName);
                        add.Add("@RoleId", request.user.RoleId);
                        add.Add("@Password", HashPass.GetSHA1HashData(request.user.Password));

                        var Add = Convert.ToInt32(await con.ExecuteAsync("AddUser", add, commandType: CommandType.StoredProcedure));
                        if (Add > 0)
                        {
                            LogRespository.RecordLog(request.currUserId, request.currEmail, 1, Add, 0, context);
                            return new BaseResponse
                            {
                                status = ResponseStatus.Success,
                                message = "Đã thêm tài khoản thành công"
                            };
                        }
                        return new BaseResponse
                        {
                            status = ResponseStatus.Fail,
                            message = "Không thể thêm tài khoản"
                        };
                    }
                }
            }
            catch (Exception ex) { return new BaseResponse { message = ex.Message }; }
        }

        public async Task<UserListReponse> AllUser(BaseRequest request)
        {
            try
            {
                using (var con = context.CreateConnection())
                {
                    var role = new DynamicParameters();
                    role.Add("@RoleId", request.UserRole);

                    var list = con.Query<UserResponse>("GetAllUser",role,commandType:CommandType.StoredProcedure).ToList();
                    return new UserListReponse
                    {
                        status = ResponseStatus.Success,
                        users = list
                    };

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BaseResponse> DeleteUser(UserDelete delete)
        {
            try
            {
                using (var con = context.CreateConnection())
                {
                    var del = new DynamicParameters();
                    del.Add("@UserId", delete.id);
                    int deleteUser = Convert.ToInt32(await con.ExecuteAsync("DeleteUser", del, commandType: CommandType.StoredProcedure));
                    if (deleteUser > 0)
                    {
                        var delassign = new DynamicParameters();
                        delassign.Add("@UserId", delete.id);
                        int delAssignee = Convert.ToInt32(await con.ExecuteAsync("DeleteAssigneeByUserId", delassign, commandType: CommandType.StoredProcedure));
                        if (delAssignee > 0)
                        {
                            LogRespository.RecordLog(delete.currUserId, delete.currEmail, 6, delete.id, 0, context);
                            LogRespository.RecordLog(delete.currUserId, delete.currEmail, 3, delete.id, 0, context);
                            return new BaseResponse { message = "Đã xóa user và assignee", status = ResponseStatus.Success };
                        }
                        LogRespository.RecordLog(delete.currUserId, delete.currEmail, 3, delete.id, 0, context);
                        return new BaseResponse { message = "Đã xóa user", status = ResponseStatus.Success };
                    }
                    return new BaseResponse { message = "Không thể xóa user", status = ResponseStatus.Fail };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse { message = ex.Message };
            }
        }

        public async Task<IEnumerable<GetUserResponse>> GetAllUser(GetUserRequest request)
        {
            try
            {
                using (var con = context.CreateConnection())
                {
                    var get = new DynamicParameters();
                    get.Add("@PageNumber", request.PageNumber);
                    get.Add("@PageSize", request.PageSize);
                    get.Add("@Email", request.Email);
                    get.Add("@FirstName", request.FirstName);
                    get.Add("@LastName", request.LastName);
                    get.Add("@RoleId", request.RoleId == 0 ? null : request.RoleId <= request.UserRole);
                    get.Add("@IsActive",request.IsActive == -1 ? null : request.IsActive);

                    var getList = await con.QueryAsync<GetUserResponse>("GetUser", get, commandType: CommandType.StoredProcedure);
                    return getList.ToList();
                }

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<UserTaskList> GetTaskAssignList()
        {
            try
            {
                using (var con = context.CreateConnection())
                {
                    var list = con.Query<TaskForUser>("GetTaskAssignList").ToList();
                    return new UserTaskList
                    {
                        status = ResponseStatus.Success,
                        usersTask = list
                    };

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BaseResponse> UpdateUser(UserRequest request)
        {
            try
            {
                using (var con = context.CreateConnection())
                {
                    var check = new DynamicParameters();
                    check.Add("@UserId", request.user.UserId);
                    check.Add("@Email", request.user.Email);
                    var checkUser = await con.QueryAsync("CheckUpdateAddUser", check, commandType: CommandType.StoredProcedure);
                    if(checkUser.Count() > 0)
                    {
                        return new BaseResponse { status = ResponseStatus.Fail, message = "Email này đã tồn tại. Không thể update email này!" };
                    }

                    var update = new DynamicParameters();
                    update.Add("@Email", request.user.Email);
                    update.Add("@FirstName", request.user.FirstName);
                    update.Add("@LastName", request.user.LastName);
                    update.Add("@RoleId", request.user.RoleId);
                    update.Add("@UserId", request.user.UserId);

                    int row = Convert.ToInt32(await con.ExecuteAsync("UpdateUser", update, commandType: CommandType.StoredProcedure));

                    if (row > 0)
                    {
                        LogRespository.RecordLog(request.currUserId, request.currEmail, 2, (int)request.user.UserId, 0, context);
                        return new BaseResponse { status = ResponseStatus.Success, message = "Update User thành công" };
                    }
                    else
                    {
                        return new BaseResponse { status = ResponseStatus.Fail, message = "Không thể update User" };
                    }
                }
            }
            catch (Exception ex) { return new BaseResponse { message = ex.Message }; }
        }
    }
}
