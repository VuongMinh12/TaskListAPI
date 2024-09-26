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
        public async Task<IEnumerable<StatusResponse>> GetStatus(StatusRequest status)
        {
            try {
                using ( var con = dapperContext.CreateConnection())
                {
                    var param = new DynamicParameters();
                    param.Add("@PageNumber", status.PageNumber);
                    param.Add("@PageSize", status.PageSize);
                    param.Add("@StatusName", status.StatusName);
                    param.Add("@IsActive", status.IsActive);
                
                    var statusList = await con.QueryAsync<StatusResponse>("GetStatus",param,commandType:CommandType.StoredProcedure);
                    return statusList.ToList();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
