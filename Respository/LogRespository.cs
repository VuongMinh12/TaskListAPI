using Dapper;
using System.Data;
using TaskListAPI.Model;

namespace TaskListAPI.Respository
{
    public static class LogRespository
    {
        public static async void RecordLog(int currUserId,string currEmail, int type, int task, int user, DapperContext dapperContext)
        {
            try
            {
                using ( var con = dapperContext.CreateConnection())
                {
                    var param = new DynamicParameters();

                    string content = "User " + currEmail;


                    switch (type)
                    {
                        case 1:
                            {
                                content += " add";
                                break;
                            }
                        case 2:
                            {
                                content += " update";
                                break;
                            }
                        case 3:
                            {
                                content += " delete";
                                break;
                            }
                        case 4:
                            {
                                content += " assignee";
                                break;
                            }
                        case 5:
                            {
                                content += " add và assignee";
                                break;
                            }
                        case 6:
                            {
                                content += " xóa assignee";
                                break;
                            }
                    }

                    if (task != 0 && user != 0) 
                    {
                        content += " Userid " + user + " tai Taskid " + task;
                        
                        param.Add("TargetTask", task);
                        param.Add("@TargetUser", user);
                    }
                    else 
                    {
                        if (user != 0)
                        {
                            content += " userId " + user;
                            param.Add("@TargetTask", task == 0 ? null : task);
                            param.Add("@TargetUser", user);
                        }
                        else
                        {
                            content += " task Id " + task;
                            param.Add("@TargetTask", task);
                            param.Add("@TargetUser", user == 0 ? null : user);
                        }

                    }
                   
                    param.Add("@Content", content);
                    param.Add("@Type", type);
                    param.Add("@Time", DateTime.Now);
                    param.Add("@UserId", currUserId);

                    var recordHis = await con.ExecuteAsync("RecordHistory", param, commandType: CommandType.StoredProcedure);


                }

            }catch (Exception e) { throw new Exception ( e.Message) ; } 
        }
    }
}
