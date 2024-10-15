using Azure.Core;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Reflection.Metadata.Ecma335;
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

        public async Task<BaseResponse> AddTask(TaskRequest request)
        {
            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    var addT = new DynamicParameters();
                    addT.Add("@Title", request.Title);
                    addT.Add("@StatusId", request.StatusId);
                    addT.Add("@CreateDate", request.CreateDate);
                    addT.Add("@FinishDate", request.FinishDate);
                    addT.Add("@Estimate", request.Estimate);
                    var taskId = Convert.ToInt32(await con.ExecuteScalarAsync("AddTask", addT, commandType: CommandType.StoredProcedure));
                    if (taskId > 0)
                    {
                        var addA = new DynamicParameters();
                        addA.Add("TaskId", taskId);
                        addA.Add("UserId", request.currUserId);
                        var AssId = Convert.ToInt32(await con.ExecuteScalarAsync("AddAssignee", addA, commandType: CommandType.StoredProcedure));
                        if (AssId > 0)
                        {
                            return new BaseResponse
                            {
                                message = "Đã thêm task thành công",
                                status = ResponseStatus.Success
                            };
                        }
                        else
                        {
                            return new BaseResponse
                            {
                                message = "Chưa thể thêm task",
                                status = ResponseStatus.Fail
                            };
                        }
                    }
                    else
                    {
                        return new BaseResponse
                        {
                            message = "Chưa thể thêm task",
                            status = ResponseStatus.Fail
                        };
                    }
                }
            }
            catch (Exception ex) { return new BaseResponse { message = ex.Message }; }
        }

        public async Task<BaseResponse> UpdateAssignee(UpdateAssignee assignee)
        {
            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    var delete = new DynamicParameters();
                    delete.Add("@TaskId", assignee.TaskId);
                    con.ExecuteAsync("DeleteAssigneeByTaskId", delete, commandType: CommandType.StoredProcedure);

                    for (int i = 0; i < assignee.UserId.Count(); i++)
                    {
                        var update = new DynamicParameters();
                        update.Add("@TaskId", assignee.TaskId);
                        update.Add("@UserId", assignee.UserId[i]);
                        await con.QueryFirstAsync("AddAssignee", update, commandType: CommandType.StoredProcedure);
                    }

                    return new BaseResponse
                    {
                        message = "Assign thành công",
                        status = ResponseStatus.Success
                    };
                   
                }

            }
            catch (Exception ex)
            {
            return new BaseResponse
                {
                    message = "Chưa thể assignee\n"+ ex.Message,
                    status = ResponseStatus.Fail
                };
            }
        }

        public async Task<IEnumerable<TaskResponse>> GetTask(GetTaskRequest request)
        {
            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    var get = new DynamicParameters();
                    get.Add("@PageNumber", request.PageNumber);
                    get.Add("@PageSize", request.PageSize);
                    get.Add("@Title", String.IsNullOrEmpty(request.Title) ? null : request.Title);
                    get.Add("@StatusId", request.StatusId == 0 ? null : request.StatusId);
                    get.Add("@CreateDate", String.IsNullOrEmpty(request.CreateDate) ? null : request.CreateDate);
                    get.Add("@FinishDate", String.IsNullOrEmpty(request.FinishDate) ? null : request.FinishDate);
                    get.Add("@UserId", request.currUserId == 0 ? null : request.currUserId);

                    var getTask = await con.QueryAsync<TaskResponse>("GetTask",get, commandType: CommandType.StoredProcedure);
                    
                    return getTask.ToList();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<BaseResponse> DeleteTask (int taskId)
        {
            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    var deleteA = new DynamicParameters();
                    deleteA.Add("@TaskId", taskId);
                    int TaskAssignee = await con.ExecuteAsync("DeleteAssigneeByTaskId", deleteA, commandType: CommandType.StoredProcedure);
                    if (TaskAssignee > 0) 
                    {
                        var param = new DynamicParameters();
                        param.Add("@TaskId", taskId);
                        int rowsAffected = await con.ExecuteAsync("DeleteTask", param, commandType: CommandType.StoredProcedure);
                        if (rowsAffected > 0)
                        {
                            return new BaseResponse
                            {
                                status = ResponseStatus.Success,
                                message = "Đã xóa task"
                            };
                        }
                        return new BaseResponse
                        {
                            status = ResponseStatus.Fail,
                            message = "Không thể xóa task!"
                        };
                    }
                    return new BaseResponse
                    {
                        status = ResponseStatus.NotAllow,
                        message = "Không được phép xóa!"
                    };
                }
            }
            catch (Exception ex) { return new BaseResponse { message = ex.Message }; }
        }

    }
}
