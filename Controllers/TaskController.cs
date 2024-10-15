using Azure.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskListAPI.Interface;
using TaskListAPI.Model;

namespace TaskListAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRespository taskRespository;
        public TaskController(ITaskRespository respository)
        {
            this.taskRespository = respository;
        }

        [HttpPost]
        public async Task<BaseResponse> AddTask(TaskRequest task)
        {
            try
            {
                return await taskRespository.AddTask(task);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        [HttpPut]
        public async Task<BaseResponse> UpdateAssignee(UpdateAssignee assignee)
        {
            try
            {
                return await taskRespository.UpdateAssignee(assignee);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        [HttpGet]
        public async Task<IEnumerable<TaskResponse>> GetTask([FromQuery] GetTaskRequest request)
        {
            
            try
            {
                var Task = await taskRespository.GetTask(request);
                return Task;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        [HttpDelete]
        public async Task<BaseResponse> DeleteTask(int id)
        {
            try
            {
                return await taskRespository.DeleteTask(id);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
