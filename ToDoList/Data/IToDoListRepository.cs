using ToDoList.Models;

namespace ToDoList.Data
{
    public interface IToDoListRepository
    {
        public Task CreateToDoListAsync(string userId, bool ishidden, string listName);
        public Task<int> CopyToDoListAsync(int id);
        public Task UpdateToDoListByIdAsync(int id, bool ishidden, string listName);
        public Task DeleteToDoListByIdAsync(int id);
        Task<List<ToDoListModel>> GetAllToDoListsByuserId(string userId);
        public Task<List<ToDoListModel>> GetAllTodayToDoListsByuserId(string userId);
        public Task<ToDoListModel> GetToDoListByListId(int id);
        public Task ChangeToDoListHiddenStatus(string userId, bool status);
    }
}
