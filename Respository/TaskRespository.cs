using Azure.Core;
using Dapper;
using System.Data;
using TaskListAPI.Interface;
using TaskListAPI.Model;

namespace TaskListAPI.Respository
{
    public class TaskRespository : ITaskRespository
    {
        private readonly DapperContext dapperContext;
        public TaskRespository(DapperContext _dapperContext)
        {
            this.dapperContext = _dapperContext;
        }

        public async Task<BaseResponse> AddTask(TaskAddUpRequest request)
        {
            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    var param = new DynamicParameters();
                    param.Add("@Title", request.task.Title);
                    param.Add("@StatusId", request.task.StatusId);
                    param.Add("@CreateDate", request.task.CreateDate);
                    param.Add("@FinishDate", request.task.FinishDate);
                    param.Add("@Estimate", request.task.Estimate);
                    param.Add("@UserId", request.currUserId);
                    
                    request.task.TaskId = Convert.ToInt32(await con.ExecuteScalarAsync("AddTask", param, commandType: CommandType.StoredProcedure));
                    if (request.task.TaskId > 0)
                    {
                        return new BaseResponse
                        {
                            status = ResponseStatus.Success,
                            message = "Them thanh cong"
                        };
                    }

                    return new BaseResponse
                    {
                        status = ResponseStatus.Fail,
                        message = "Them ko thanh cong"
                    }; 
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<BaseResponse> DeleteTask(TaskDelete request)
        {
            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    var param = new DynamicParameters();
                    param.Add("@TaskId", request.id);
                    int rowsAffected = await con.ExecuteAsync("DeleteTask", param, commandType: CommandType.StoredProcedure);
                    if (rowsAffected > 0)
                    {
                        return new BaseResponse
                        {
                            status = ResponseStatus.Success,
                            message = "xoa thanh cong"
                        };
                    }

                    return new BaseResponse
                    {
                        status = ResponseStatus.Fail,
                        message = "Khong the xoa"
                    }; 
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<TaskResponse>> GetTask(TaskRequest request)
        {
            try
            {
                using (var con = dapperContext.CreateConnection())
                {

                    var param = new DynamicParameters();

                    param.Add("@PageNumber", request.PageNumber);
                    param.Add("@PageSize", request.PageSize);
                    param.Add("@Title", String.IsNullOrEmpty(request.Title) ? null : request.Title);
                    param.Add("@StatusId", request.StatusId == 0 ? null : request.StatusId);
                    param.Add("@CreateDate", String.IsNullOrEmpty(request.CreateDate) ? null : request.CreateDate);
                    param.Add("@FinishDate", String.IsNullOrEmpty(request.FinishDate) ? null : request.FinishDate);
                    param.Add("@UserId", request.currUserId == 0 ? null : request.currUserId);

                    var todo = await con.QueryAsync<TaskResponse>("GetTask", param, commandType: CommandType.StoredProcedure);

                    return todo.ToList();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<BaseResponse> UpdateTask(TaskAddUpRequest request)
        {

            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    var param = new DynamicParameters();
                    param.Add("@Title", request.task.Title);
                    param.Add("@StatusId", request.task.StatusId);
                    param.Add("@CreateDate", request.task.CreateDate);
                    param.Add("@FinishDate", request.task.FinishDate);
                    param.Add("@Estimate", request.task.Estimate);
                    param.Add("@UserId", request.currUserId);
                    param.Add("@TaskId", request.task.TaskId);

                    int rowsAffected = await con.ExecuteAsync("UpdateTask", param, commandType: CommandType.StoredProcedure);
                    if( rowsAffected > 0)
                    {
                        return new BaseResponse
                        {
                            status = ResponseStatus.Success,
                            message = "Cap nhat thanh cong"
                        };
                    }

                    return new BaseResponse
                    {
                        status = ResponseStatus.Fail,
                        message = "Cap nhat ko thanh cong"
                    };
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    } 
}
