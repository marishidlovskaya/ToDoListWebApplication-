using ToDoEntities;
using ToDoList.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ToDoList.Data
{
    
    public class ToDoListRepository : IToDoListRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public ToDoListRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task CreateToDoListAsync(string userId, bool ishidden, string listName)
        {
            if (listName != null)
            {
                ToDoEntities.ToDoList toDoList = new ToDoEntities.ToDoList()
                {
                    Name = listName,
                    UserId = userId,
                    isListHidden = ishidden,
                    DateCreated = DateTime.Now,
                };
                await _applicationDbContext.AddAsync(toDoList);
                await _applicationDbContext.SaveChangesAsync();
            }
        }

        public async Task<int> CopyToDoListAsync(int id)
        {
            var list = await _applicationDbContext.ToDoLists.FirstOrDefaultAsync(x => x.Id == id);
            if(list == null) throw new ArgumentNullException(nameof(list), "List with id " + id + " does not exist");

            var newToDoList = new ToDoEntities.ToDoList()
            {
                Name = list.Name,
                DateCreated = DateTime.Now,
                isListHidden = list.isListHidden,
                UserId = list.UserId,
            };
            await _applicationDbContext.AddAsync(newToDoList);
            await _applicationDbContext.SaveChangesAsync();

            foreach (var item in await _applicationDbContext.ToDoItems.Where(x => x.ToDoListId == id).ToListAsync())
            {
                await _applicationDbContext.AddAsync(new ToDoItem()
                {
                    Name = item.Name,
                    Description = item.Description,
                    DateCreated = item.DateCreated,
                    DueDate = item.DueDate,
                    DateReminder = item.DateReminder,
                    Status = item.Status,
                    ToDoListId = newToDoList.Id,
                });
                await _applicationDbContext.SaveChangesAsync();
            }
            return newToDoList.Id;
        }

        public async Task UpdateToDoListByIdAsync(int id, bool ishidden, string listName)
        {
            var list = await _applicationDbContext.ToDoLists.FirstOrDefaultAsync(x => x.Id == id);
            if (list == null) throw new ArgumentNullException(nameof(list), "List with id " + id + " does not exist");
            list.Name = listName;
            list.isListHidden = ishidden;
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task DeleteToDoListByIdAsync(int id)
        {
            var list = await _applicationDbContext.ToDoLists.FindAsync(id);
            _applicationDbContext.ToDoLists.Remove(list);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<List<ToDoListModel>> GetAllToDoListsByuserId(string userId)
        {
            var res = await _applicationDbContext.ToDoLists.Where(x => x.UserId == userId)
                .Select(s => new ToDoListModel()
                {
                    Id = s.Id,
                    Name = s.Name,
                    UserId = s.UserId,
                    DateCreated = s.DateCreated,
                    isListHidden = s.isListHidden,
                    toDoItems = _applicationDbContext.ToDoItems.Where(f => f.ToDoListId == s.Id)
                    .Select(f => new ToDoItemModel()
                    {
                        Id = f.Id,
                        DateCreated = f.DateCreated,
                        Status = f.Status,
                        DateReminder = f.DateReminder,
                        Description = f.Description,
                        DueDate = f.DueDate,
                        Name = f.Name,
                        ToDoListId = f.ToDoListId,
                        Note = f.Note,

                    }).ToList(),
                })
                .OrderByDescending(f => f.DateCreated).ToListAsync();
            return res;
        }

        public async Task<List<ToDoListModel>> GetAllTodayToDoListsByuserId(string userId)
        {
            var res = await _applicationDbContext.ToDoLists.Where(x => x.UserId == userId && _applicationDbContext.ToDoItems.Where(g => g.ToDoList == x).Count() > 0)
           .Select(s => new ToDoListModel()
           {
               Id = s.Id,
               Name = s.Name,
               UserId = s.UserId,
               DateCreated = s.DateCreated,
               isListHidden = s.isListHidden,
               toDoItems = _applicationDbContext.ToDoItems.Where(f => f.ToDoListId == s.Id && f.DueDate == DateTime.Today)

                    .Select(f => new ToDoItemModel()
                    {
                        Id = f.Id,
                        DateCreated = f.DateCreated,
                        Status = f.Status,
                        DateReminder = f.DateReminder,
                        Description = f.Description,
                        DueDate = f.DueDate,
                        Name = f.Name,
                        ToDoListId = f.ToDoListId,
                        Note = f.Note,
                    }).ToList(),
           }).Where(f => f.toDoItems.Count > 0).ToListAsync();

            return res;

        }

        public async Task<ToDoListModel> GetToDoListByListId(int id)
        {
            var list = await _applicationDbContext.ToDoLists.Where(x => x.Id == id)
                 .Select(s => new ToDoListModel()
                 {
                     Id = s.Id,
                     Name = s.Name,
                     UserId = s.UserId,
                     DateCreated = s.DateCreated,
                     isListHidden = s.isListHidden,
                     toDoItems = _applicationDbContext.ToDoItems.Where(f => f.ToDoListId == s.Id)
                    .Select(f => new ToDoItemModel()
                    {
                        Id = f.Id,
                        DateCreated = f.DateCreated,
                        Status = f.Status,
                        DateReminder = f.DateReminder,
                        Description = f.Description,
                        DueDate = f.DueDate,
                        Name = f.Name,
                        ToDoListId = f.ToDoListId,
                        Note = f.Note,
                    }).ToList(),
                 }).FirstOrDefaultAsync();
            return list;
        }

        public async Task ChangeToDoListHiddenStatus(string userId, bool status)
        {
            User user = await _applicationDbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            if (user == null) throw new ArgumentNullException(nameof(userId), "User with id " + userId + " does not exist");
            user.ShowHiddenLists = status;
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
