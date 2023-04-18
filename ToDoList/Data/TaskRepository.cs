using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ToDoEntities;
using ToDoList.Models;

namespace ToDoList.Data
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public TaskRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<ToDoItemModel>> GetAllItemsByListIdAsync(int id)
        {
            var res = await _applicationDbContext.ToDoItems.Where(x => x.ToDoListId == id)
                .Select(s => new ToDoItemModel()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    DateCreated = s.DateCreated,
                    DateReminder = s.DateReminder,
                    DueDate = s.DueDate,
                    ToDoListId = s.ToDoListId,
                    Status = s.Status,
                    Note = s.Note,
                })
                .OrderByDescending(f => f.DateCreated).ToListAsync();
            return res;
        }

        public async Task<List<ToDoItemModel>> GetAllItemsByListId(int id)
        {
            var listName = await _applicationDbContext.ToDoLists.Where(x => x.Id == id).Select(f => f.Name).FirstOrDefaultAsync();
            var res = await _applicationDbContext.ToDoItems.Where(x => x.ToDoListId == id)
                .Select(s => new ToDoItemModel()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    DateCreated = s.DateCreated,
                    DateReminder = s.DateReminder,
                    DueDate = s.DueDate,
                    ToDoListId = s.ToDoListId,
                    Status = s.Status,
                    Note = s.Note,
                })
                .OrderByDescending(f => f.DateCreated).ToListAsync();
            return res;
        }

        public async Task<ToDoItemModel> GetItemAsync(int id)
        {
            var item = await _applicationDbContext.ToDoItems.Where(x => x.Id == id).Select(s => new ToDoItemModel()
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                DateCreated = s.DateCreated,
                DateReminder = s.DateReminder,
                DueDate = s.DueDate,
                ToDoListId = s.ToDoListId,
                Status = s.Status,
                Note = s.Note,
            }).FirstOrDefaultAsync();

            return item;
        }

        public async Task ChangeTaskStatusAsync(int taskId, string status)
        {
            var todoItem = await _applicationDbContext.ToDoItems.Where(x => x.Id == taskId).FirstOrDefaultAsync();
            if (todoItem == null) throw new ArgumentNullException(nameof(todoItem), "task with id " + taskId + " does not exist");
            todoItem.Status = status;
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task DeleteTaskByIdAsync(int taskId)
        {
            var taskToDelete = await _applicationDbContext.ToDoItems.Where(x => x.Id == taskId).FirstOrDefaultAsync();
            if (taskToDelete == null) throw new ArgumentNullException(nameof(taskToDelete), "task with id " + taskId + " does not exist");
            _applicationDbContext.Remove(taskToDelete);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task AddItemAsync(ToDoItemModel toDoItemModel)
        {
            if (toDoItemModel != null)
            {
                ToDoItem toDoItem = new ToDoItem()
                {
                    DateCreated = DateTime.Now,
                    DateReminder = toDoItemModel.DateReminder,
                    Description = toDoItemModel.Description,
                    DueDate = toDoItemModel.DueDate,
                    Status = toDoItemModel.Status,
                    Name = toDoItemModel.Name,
                    Note = toDoItemModel.Note,
                    ToDoListId = toDoItemModel.ToDoListId,
                };
                await _applicationDbContext.AddAsync(toDoItem);
                await _applicationDbContext.SaveChangesAsync();
            }         
        }

        public async Task UpdateItemAsync(ToDoItemModel toDoItemModel)
        {
            if (toDoItemModel != null)
            {
                var item = await _applicationDbContext.ToDoItems.Where(x => x.Id == toDoItemModel.Id).FirstOrDefaultAsync();
                item.Note = toDoItemModel.Note;
                item.DueDate = toDoItemModel.DueDate;
                item.Status = toDoItemModel.Status;
                item.Name = toDoItemModel.Name;
                item.ToDoListId = toDoItemModel.ToDoListId;
                item.Id = toDoItemModel.Id;
                item.Description = toDoItemModel.Description;
                item.DateCreated = toDoItemModel.DateCreated;
                item.DateReminder = toDoItemModel.DateReminder;

                await _applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
