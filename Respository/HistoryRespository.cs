﻿using Azure.Core;
using Dapper;
using System.Data;
using TaskListAPI.Interface;
using TaskListAPI.Model;

namespace TaskListAPI.Respository
{
    public static class HistoryRespository
    {
        public static async void RecordLog(int currUserId, string currUserName, int type, int target, bool isTask, DapperContext dapperContext)
        {
            try
            {
                using (var con = dapperContext.CreateConnection())
                {
                    string content = "Người dùng " + currUserName;

                    switch (type)
                    {
                        case 1:
                            {
                                content += " add";
                                break;
                            }
                        case 2:
                            {
                                content += " edit";
                                break;
                            }
                        case 3:
                            {
                                content += " delete";
                                break;
                            }
                        case 4:
                            {
                                content += " đổi mật khẩu";
                                break;
                            }
                    }

                    var param = new DynamicParameters();

                    if (isTask)
                    {
                        content += " task id " + target;
                        param.Add("TargetTask", target);
                        param.Add("@TargetUser", 0);
                    }
                    else
                    {
                        content += " user id " + target;
                        param.Add("TargetTask", 0);
                        param.Add("@TargetUser", target);
                    }

                    param.Add("@Content", content);
                    param.Add("Type", type);
                    param.Add("@Time", DateTime.Now);
                    param.Add("@UserId", currUserId);
                    
                    var recordHis = await con.ExecuteAsync("RecordHistory", param, commandType: CommandType.StoredProcedure);
                    
                }
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }
    }
}
