using Azure.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskListAPI.Interface;
using TaskListAPI.Model;
using static TaskListAPI.Model.TaskAddUpdateRequest;

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
        public async Task<BaseResponse> AddTask(TaskAddUpdateRequest request)
        {
            try
            {
                return await taskRespository.AddTask(request);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        [HttpPut]
        public async Task<BaseResponse> UpdateTask(TaskAddUpdateRequest request)
        {
            try
            {
                return await taskRespository.UpdateTask(request);
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
        public async Task<BaseResponse> DeleteTask(TaskDelete delete)
        {
            try
            {
                return await taskRespository.DeleteTask(delete);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
