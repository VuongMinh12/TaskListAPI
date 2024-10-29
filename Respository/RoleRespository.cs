using Dapper;
using TaskListAPI.Interface;
using TaskListAPI.Model;
using System.Data;
using Azure.Core;

namespace TaskListAPI.Respository
{
    public class RoleRespository : IRoleRespository
    {
        private readonly DapperContext context;
        public RoleRespository (DapperContext context)
        {
            this.context = context;
        }

        public async Task<BaseResponse> AddRole(RequestRoleAddUp request)
        {
            try
            {
                using (var con = context.CreateConnection())
                {
                    var update = new DynamicParameters();
                    update.Add("@RoleName", request.role.RoleName);

                    int row = Convert.ToInt32(await con.ExecuteAsync("AddRole", update, commandType: CommandType.StoredProcedure));

                    if (row > 0)
                    {
                        return new BaseResponse { status = ResponseStatus.Success, message = "Thêm Role thành công" };
                    }
                    else
                    {
                        return new BaseResponse { status = ResponseStatus.Fail, message = "Không thể thêm Role" };
                    }
                }
            }
            catch (Exception ex) { return new BaseResponse { message = ex.Message }; }
        }

        public async Task<BaseResponse> DeleteRole(RoleDelete delete)
        {
            try
            {
                using (var con = context.CreateConnection())
                {
                    var check = new DynamicParameters();
                    check.Add("@RoleId", delete.id);
                    var list = await con.QueryAsync("CheckRoleinUser", check, commandType: CommandType.StoredProcedure);
                    if (list.Count() > 0)
                    {
                        return new BaseResponse { message = "Đang có user ở role này, không được phép xóa!", status = ResponseStatus.Fail };
                    }

                    var del = new DynamicParameters();
                    del.Add("@RoleId", delete.id);
                    int deleteRole = Convert.ToInt32(await con.ExecuteAsync("DeleteRole", del, commandType: CommandType.StoredProcedure));
                    if (deleteRole > 0)
                    {
                        return new BaseResponse { message = "Đã xóa role", status = ResponseStatus.Success };
                    }
                    return new BaseResponse { message = "Không thể xóa role", status = ResponseStatus.Fail };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse { message = ex.Message };
            }
        }

        public async Task<IEnumerable<RoleRespone>> GetRole(RoleRequest request)
        {
            try
            {
                using (var con = context.CreateConnection()) 
                {
                    var get = new DynamicParameters();
                    get.Add("@PageNumber", request.PageNumber);
                    get.Add("@PageSize", request.PageSize);
                    get.Add("@RoleName", request.RoleName);
                    get.Add("@IsActive", request.IsActive == -1 ? null : request.IsActive);
                    get.Add("@CurrenRole",request.UserRole);

                    var allRole = await con.QueryAsync<RoleRespone>("GetRole", get, commandType:CommandType.StoredProcedure); 
                    return allRole.ToList();
                }
            } catch (Exception e) { throw new Exception(e.Message); }
        }

        public async Task<BaseResponse> UpdateRole(RequestRoleAddUp request)
        {
            try
            {
                using (var con = context.CreateConnection())
                {
                    var update = new DynamicParameters();
                    update.Add("@RoleName", request.role.RoleName);
                    update.Add("@RoleId", request.role.RoleId);

                    int row = Convert.ToInt32(await con.ExecuteAsync("UpdateRole", update, commandType: CommandType.StoredProcedure));

                    if (row > 0)
                    {
                        return new BaseResponse { status = ResponseStatus.Success, message = "Update role thành công" };
                    }
                    else
                    {
                        return new BaseResponse { status = ResponseStatus.Fail, message = "Không thể update role" };
                    }
                }
            }
            catch (Exception ex) { return new BaseResponse { message = ex.Message }; }
        }
    }
}
