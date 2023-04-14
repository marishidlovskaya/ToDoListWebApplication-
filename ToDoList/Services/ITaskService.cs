using ToDoList.Models;

namespace ToDoList.Services
{
    public interface ITaskService
    {
        public Task<List<ToDoItemModel>> GetAllItemsByListIdAsync(int id);
        public Task<List<ToDoItemModel>> GetAllItemsByListId(int id);
        public Task<ToDoItemModel> GetItemAsync(int id);
        public Task ChangeTaskStatusAsync(int taskId, string status);
        public Task DeleteTaskByIdAsync(int taskId);
        public Task AddItemAsync(ToDoItemModel toDoItemModel);
        public Task UpdateItemAsync(ToDoItemModel toDoItemModel);
    }
}
