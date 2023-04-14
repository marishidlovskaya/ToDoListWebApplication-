using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskService;
        public TaskService(ITaskRepository taskService)
        {
            _taskService = taskService;
        }

        public async Task AddItemAsync(ToDoItemModel toDoItemModel)
        {
            await _taskService.AddItemAsync(toDoItemModel);
        }

        public async Task ChangeTaskStatusAsync(int taskId, string status)
        {
            await _taskService.ChangeTaskStatusAsync(taskId, status);
        }

        public async Task DeleteTaskByIdAsync(int taskId)
        {
            await _taskService.DeleteTaskByIdAsync(taskId);
        }

        public async Task<List<ToDoItemModel>> GetAllItemsByListId(int id)
        {
            return await _taskService.GetAllItemsByListId(id);
        }

        public async Task<List<ToDoItemModel>> GetAllItemsByListIdAsync(int id)
        {
            return await _taskService.GetAllItemsByListIdAsync(id);
        }

        public async Task<ToDoItemModel> GetItemAsync(int id)
        {
            return await _taskService.GetItemAsync(id);
        }

        public async Task UpdateItemAsync(ToDoItemModel toDoItemModel)
        {
            await _taskService.UpdateItemAsync(toDoItemModel);
        }
    }
}
