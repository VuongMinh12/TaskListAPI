using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskListAPI.Interface;
using TaskListAPI.Model;

namespace TaskListAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRespository taskRespository;
        public TaskController(ITaskRespository respository)
        {
            this.taskRespository = respository;
        }

        [HttpGet]
        public async Task<IEnumerable<TaskResponse>> GetTask([FromQuery] TaskRequest request)
        {
            var Task = await taskRespository.GetTask(request);
            return Task;
        }

        [HttpPost]
        public async Task<int> AddTask(TaskResponse task)
        {
            try
            {
                return await taskRespository.AddTask(task);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        [HttpPut]
        public async Task<bool> UpdateTask(TaskResponse task)
        {
            try
            {
                return await taskRespository.UpdateTask(task);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        [HttpDelete]
        public async Task<bool> DeleteTask(int id)
        {
            try
            {
                return await taskRespository.DeleteTask(id);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
