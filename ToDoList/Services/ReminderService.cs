using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoEntities;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Services
{
    public class ReminderService : IReminderService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public ReminderService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public List<ToDoItemModel> GetRemindItems(string userId)
        {
            DateTime dateTime = DateTime.UtcNow;
            List<int> usersLisIds = new List<int>();
            usersLisIds = _applicationDbContext.ToDoLists.Where(f => f.UserId == userId).Select(s => s.Id).ToList();
            return _applicationDbContext.ToDoItems
                .Where(f => f.DateReminder >= dateTime && f.DateReminder <= dateTime.AddSeconds(5) && usersLisIds.Contains(f.ToDoListId))
                .Select(s => new ToDoItemModel()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    DateCreated = s.DateCreated,
                    DateReminder = s.DateReminder != null ? TimeZone.CurrentTimeZone.ToLocalTime((DateTime)s.DateReminder) : null,
                    DueDate = s.DueDate,
                    ToDoListId = s.ToDoListId,
                    Status = s.Status,
                    Note = s.Note,
                }).ToList();
        }

    }
}
