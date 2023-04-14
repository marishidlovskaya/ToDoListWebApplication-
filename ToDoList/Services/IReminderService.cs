using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;

namespace ToDoList.Services
{
    public interface IReminderService
    {
        public List<ToDoItemModel> GetRemindItems(string userId);

    }
}
