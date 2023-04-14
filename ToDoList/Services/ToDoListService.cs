using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Services
{
    public class ToDoListService : IToDoListService
    {
        private readonly IToDoListRepository _toDoListRepository;

        public ToDoListService(IToDoListRepository toDoListRepository)
        {
            _toDoListRepository = toDoListRepository;
        }

        public async Task CreateToDoListAsync(string userId, bool ishidden, string listName)
        {
            await _toDoListRepository.CreateToDoListAsync(userId, ishidden, listName);
        }

        public async Task<int> CopyToDoListAsync(int id)
        {
            return await _toDoListRepository.CopyToDoListAsync(id);
        }

        public async Task UpdateToDoListByIdAsync(int id, bool ishidden, string listName)
        {
            await _toDoListRepository.UpdateToDoListByIdAsync(id, ishidden, listName);
        }

        public async Task DeleteToDoListByIdAsync(int id)
        {
            await _toDoListRepository.DeleteToDoListByIdAsync(id);
        }

        public async Task<List<ToDoListModel>> GetAllToDoListsByuserId(string userId)
        {
            return await _toDoListRepository.GetAllToDoListsByuserId(userId);
        }

        public async Task<List<ToDoListModel>> GetAllTodayToDoListsByuserId(string userId)
        {
            return await _toDoListRepository.GetAllTodayToDoListsByuserId(userId);
        }

        public async Task<ToDoListModel> GetToDoListByListId(int id)
        {
            return await _toDoListRepository.GetToDoListByListId(id);
        }

        public async Task<bool> isToDoListHidden(int id)
        {
            var toDoList = await _toDoListRepository.GetToDoListByListId(id);
            return toDoList.isListHidden;
        }

        public async Task ChangeToDoListHiddenStatus(string userId, bool status)
        {
            await _toDoListRepository.ChangeToDoListHiddenStatus(userId, status);
        }
    }
}
