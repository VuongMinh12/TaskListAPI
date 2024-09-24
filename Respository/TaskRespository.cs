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

        public async Task<int> AddTask(TaskResponse task)
        {
            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    var param = new DynamicParameters();
                    param.Add("@Title", task.Title);
                    param.Add("@StatusId", task.StatusId);
                    param.Add("@CreateDate", task.CreateDate);
                    param.Add("@FinishDate", task.FinishDate);
                    param.Add("@Estimate", task.Estimate);
                    param.Add("@UserId", task.UserId);
                    task.TaskId = Convert.ToInt32(await con.ExecuteScalarAsync("AddTask", param, commandType: CommandType.StoredProcedure));
                    return task.TaskId;
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<bool> DeleteTask(int id)
        {
            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    var param = new DynamicParameters();
                    param.Add("@TodoId", id);
                    int rowsAffected = await con.ExecuteAsync("DeleteTask", param, commandType: CommandType.StoredProcedure);
                    return rowsAffected > 0;
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

        public async Task<bool> UpdateTask(TaskResponse task)
        {

            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    var param = new DynamicParameters();
                    param.Add("@Title", task.Title);
                    param.Add("@StatusId", task.StatusId);
                    param.Add("@CreateDate", task.CreateDate);
                    param.Add("@FinishDate", task.FinishDate);
                    param.Add("@Estimate", task.Estimate);
                    param.Add("@UserId", task.UserId);

                    int rowsAffected = await con.ExecuteAsync("UpdateTask", param, commandType: CommandType.StoredProcedure);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    } 
}
