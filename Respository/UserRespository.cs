using Dapper;
using TaskListAPI.Interface;
using TaskListAPI.Model;

namespace TaskListAPI.Respository
{
    public class UserRespository : IUserRespository
    {
        private readonly DapperContext context;
        public UserRespository (DapperContext context)
        {
            this.context = context;
        }
        public async Task<UserListReponse> AllUser()
        {
            try
            {
                using (var con = context.CreateConnection()) 
                {
                    var list = con.Query<UserResponse>("GetAllUser").ToList();
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
    }
}
