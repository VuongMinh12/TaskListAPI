using Azure.Core;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
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

        public async Task<BaseResponse> AddTask(TaskAddUpdateRequest request)
        {
            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    var addT = new DynamicParameters();
                    addT.Add("@Title", request.task.Title);
                    addT.Add("@StatusId", request.task.StatusId);
                    addT.Add("@CreateDate", request.task.CreateDate);
                    addT.Add("@FinishDate", request.task.FinishDate);
                    addT.Add("@Estimate", request.task.Estimate);
                    var taskId = Convert.ToInt32(await con.ExecuteScalarAsync("AddTask", addT, commandType: CommandType.StoredProcedure));

                    if (taskId > 0)
                    {
                        if (request.UserRole < 2)
                        {
                            var addTA = new DynamicParameters();
                            addTA.Add("@TaskId", taskId);
                            addTA.Add("@UserId", request.currUserId);
                            int AddNew = Convert.ToInt32(await con.ExecuteAsync("AddAssignee", addTA, commandType: CommandType.StoredProcedure));
                            if (AddNew > 0)
                            {
                                
                                LogRespository.RecordLog(request.currUserId, request.currEmail, 5,  taskId, request.currUserId, dapperContext);
                                return new BaseResponse
                                {
                                    message = "Đã thêm task và assignee thành công",
                                    status = ResponseStatus.Success

                                };
                            }
                            return new BaseResponse
                            {
                                message = "Chưa thể thêm task",
                                status = ResponseStatus.Fail

                            };
                        }

                        var addA = new DynamicParameters();
                        var list = request.task.ListUser.Count;
                        if (list > 0)
                        {
                            for (int i = 0; i < list; i++)
                            {
                                addA.Add("@TaskId", taskId);
                                addA.Add("@UserId", request.task.ListUser[i]);
                                var AssId = await con.QuerySingleOrDefaultAsync("AddAssignee", addA, commandType: CommandType.StoredProcedure);
                                LogRespository.RecordLog(request.currUserId, request.currEmail, 5, taskId, request.task.ListUser[i], dapperContext);
                            }
                            return new BaseResponse
                            {
                                message = "Đã thêm task cùng assignee thành công",
                                status = ResponseStatus.Success

                            };
                        }
                        LogRespository.RecordLog(request.currUserId, request.currEmail, 1, taskId, 0, dapperContext);
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
            }
            catch (Exception ex) { return new BaseResponse { message = ex.Message }; }
        }

        public async Task<BaseResponse> UpdateTask(TaskAddUpdateRequest request)
        {
            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    var upT = new DynamicParameters();
                    upT.Add("@Title", request.task.Title);
                    upT.Add("@StatusId", request.task.StatusId);
                    upT.Add("@CreateDate", request.task.CreateDate);
                    upT.Add("@FinishDate", request.task.FinishDate);
                    upT.Add("@Estimate", request.task.Estimate);
                    upT.Add("@TaskId", request.task.TaskId);
                    int UpTask = await con.ExecuteAsync("UpdateTask", upT, commandType: CommandType.StoredProcedure);
                    if (UpTask > 0)
                    {
                        if (request.task.ListUser.Count >= 0 && request.UserRole > 1)
                        {
                            var delete = new DynamicParameters();
                            delete.Add("@TaskId", request.task.TaskId);
                            await con.ExecuteAsync("DeleteAssigneeByTaskId", delete, commandType: CommandType.StoredProcedure);
                           

                            for (int i = 0; i < request.task.ListUser.Count(); i++)
                            {
                                LogRespository.RecordLog(request.currUserId, request.currEmail, 6, (int)request.task.TaskId, request.task.ListUser[i], dapperContext);
                                var assign = new DynamicParameters();
                                assign.Add("@TaskId", request.task.TaskId);
                                assign.Add("@UserId", request.task.ListUser[i]);
                                await con.QuerySingleOrDefaultAsync("AddAssignee", assign, commandType: CommandType.StoredProcedure);
                                LogRespository.RecordLog(request.currUserId, request.currEmail, 4, (int)request.task.TaskId, request.task.ListUser[i], dapperContext);
                            }

                            return new BaseResponse
                            {
                                message = "Dã update Task cùng Assignee",
                                status = ResponseStatus.Success
                            };
                        }
                        else
                        {
                            LogRespository.RecordLog(request.currUserId, request.currEmail, 2, (int)request.task.TaskId, 0, dapperContext);
                            return new BaseResponse
                            {
                                message = "Dã update Task",
                                status = ResponseStatus.Success
                            };
                        }

                    }
                    return new BaseResponse
                    {
                        message = "Không thể update Task",
                        status = ResponseStatus.Fail
                    };

                }

            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    message = ex.Message,
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
                    get.Add("@Title", request.Title);
                    get.Add("@StatusId", request.StatusId == 0 ? null : request.StatusId);
                    get.Add("@CreateDate", request.CreateDate);
                    get.Add("@FinishDate", request.FinishDate);
                    get.Add("@CurrUserId", request.currUserId);

                    var getTask = await con.QueryAsync<TaskResponse>("GetTask", get, commandType: CommandType.StoredProcedure);

                    return getTask.ToList();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<BaseResponse> DeleteTask(TaskDelete delete)
        {
            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    var param = new DynamicParameters();
                    param.Add("@TaskId", delete.id);
                    int rowsAffected = await con.ExecuteAsync("DeleteTask", param, commandType: CommandType.StoredProcedure);
                    if (rowsAffected > 0)
                    {
                        var deleteA = new DynamicParameters();
                        deleteA.Add("@TaskId", delete.id);
                        int TaskAssignee = await con.ExecuteAsync("DeleteAssigneeByTaskId", deleteA, commandType: CommandType.StoredProcedure);
                        if (TaskAssignee > 0)
                        {
                            LogRespository.RecordLog(delete.currUserId, delete.currEmail, 6, delete.id, 0, dapperContext);
                            return new BaseResponse
                            {
                                status = ResponseStatus.Success,
                                message = "Đã xóa task và assignee"
                            };
                        }
                        LogRespository.RecordLog(delete.currUserId, delete.currEmail, 3, delete.id, 0, dapperContext);
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
            }
            catch (Exception ex) { return new BaseResponse { message = ex.Message }; }
        }

    }
}
