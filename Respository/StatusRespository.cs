using Azure.Core;
using Dapper;
using System.Data;
using TaskListAPI.Interface;
using TaskListAPI.Model;

namespace TaskListAPI.Respository
{
    public class StatusRespository : IStatusRespository
    {
        private readonly DapperContext dapperContext;
        public StatusRespository(DapperContext dapperContext)
        {
            this.dapperContext = dapperContext;
        }

        public async Task<BaseResponse> AddStatus(RequestStatusAddUp request)
        {
            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    var update = new DynamicParameters();
                    update.Add("@StatusName", request.status.StatusName);

                    int row = Convert.ToInt32(await con.ExecuteAsync("AddStatus", update, commandType: CommandType.StoredProcedure));

                    if (row > 0)
                    {
                        return new BaseResponse { status = ResponseStatus.Success, message = "Thêm Status thành công" };
                    }
                    else
                    {
                        return new BaseResponse { status = ResponseStatus.Fail, message = "Không thể thêm Status" };
                    }
                }
            }
            catch (Exception ex) { return new BaseResponse { message = ex.Message }; }
        }

        public async Task<BaseResponse> DeleteStatus(StatusDelete delete)
        {
            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    var check = new DynamicParameters();
                    check.Add("@StatusId", delete.id);
                    var list = await con.QueryAsync("CheckStatusinTask", check, commandType: CommandType.StoredProcedure);
                    if(list.Count() > 0)
                    {
                        return new BaseResponse { message = "Đang có task dùng status này không được phép xóa", status = ResponseStatus.Fail };
                    }

                    var del = new DynamicParameters();
                    del.Add("@StatusId", delete.id);
                    int deleteStatus = Convert.ToInt32(await con.ExecuteAsync("DeleteStatus", del, commandType: CommandType.StoredProcedure));
                    if (deleteStatus > 0)
                    {
                        return new BaseResponse { message = "Đã xóa status", status = ResponseStatus.Success };
                    }
                    return new BaseResponse { message = "Không thể xóa status", status = ResponseStatus.Fail };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse { message = ex.Message };
            }
        }

        public async Task<IEnumerable<StatusResponse>> GetStatus(StatusRequest request)
        {
            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    var param = new DynamicParameters();
                    param.Add("@PageNumber", request.PageNumber);
                    param.Add("@PageSize", request.PageSize);
                    param.Add("@StatusName", request.StatusName);
                    param.Add("@IsActive", request.IsActive == -1 ? null : request.IsActive);

                    var statusList = await con.QueryAsync<StatusResponse>("GetStatus", param, commandType: CommandType.StoredProcedure);
                    return statusList.ToList();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<BaseResponse> UpdateStatus(RequestStatusAddUp request)
        {
            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    var update = new DynamicParameters();
                    update.Add("@StatusName", request.status.StatusName);
                    update.Add("@StatusId", request.status.StatusId);

                    int row = Convert.ToInt32(await con.ExecuteAsync("UpdateStatus", update, commandType: CommandType.StoredProcedure));

                    if (row > 0)
                    {
                        return new BaseResponse { status = ResponseStatus.Success, message = "Update Status thành công" };
                    }
                    else
                    {
                        return new BaseResponse { status = ResponseStatus.Fail, message = "Không thể update Status" };
                    }
                }
            }
            catch (Exception ex){return new BaseResponse{ message = ex.Message};}
        }
    }
}



