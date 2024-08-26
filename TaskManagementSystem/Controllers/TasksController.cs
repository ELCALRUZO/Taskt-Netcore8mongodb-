using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repository.Interface;

namespace TaskManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public TasksController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        // GET /tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManagementTask>>> GetTasks()
        {
            var tasks = await _taskRepository.GetTasksAsync();
            return Ok(tasks);
        }

        // GET /tasks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ManagementTask>> GetTask(string id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        // POST /tasks
        [HttpPost]
        public async Task<ActionResult<ManagementTask>> CreateTask(ManagementTask task)
        {
            await _taskRepository.CreateTaskAsync(task);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        // PUT /tasks/{id}
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateTask(string id, ManagementTask task)
        //{
        //    //if (id != task.Id)
        //    if (id != task.Id.ToString())
        //    {
        //        return BadRequest();
        //    }

        //      await _taskRepository.UpdateTaskAsync(id, task);
        //    // return NoContent();
        //   return Ok();
        //}


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(string id, [FromBody] UpdateTaskRequest request)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest(new { Message = "Invalid task ID." });
            }

            var updatedTask = new ManagementTask
            {
                Id = objectId,
                Title = request.Task.Title,
                Description = request.Task.Description,
                Status = request.Task.Status,
                Priority = request.Task.Priority,
                ListId = request.Task.ListId,
                GroupId = request.Task.GroupId,
                AssignedUsers = request.Task.AssignedUsers,
                UpdatedAt = DateTime.UtcNow
            };

            var result =  _taskRepository.UpdateTaskAsync(objectId.ToString(), updatedTask);

            if (result==null)
            {
                return Ok(updatedTask);
            }

            return NotFound(new { Message = "Task not found." });
        }






        // DELETE /tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            await _taskRepository.DeleteTaskAsync(id);
            return NoContent();
        }



    }
}
