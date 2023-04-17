using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ToDoList.Data;
using ToDoList.Models;
using ToDoList.Services;

namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoItemController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IReminderService _reminderService;
        private readonly ITaskService _taskService;
        public ToDoItemController(ApplicationDbContext applicationDbContext, ITaskService taskService, IReminderService reminderService)
        {
            _applicationDbContext = applicationDbContext;
            _reminderService = reminderService;
            _taskService = taskService;
        }
        [Route("AddItem")]
        [HttpPost]
        public async Task<IActionResult> AddItem(ToDoItemModel toDoItemModel)
        {          
            try
            {
                await _taskService.AddItemAsync(toDoItemModel);
                return Ok("Ok");
            }
            catch
            {
                return Problem("Error during saving the task");
            }
        }

        
        [Route("UpdateItem")]
        [HttpPut]
        public async Task<IActionResult> UpdateItem(ToDoItemModel toDoItemModel)
        {
            try
            {
                await _taskService.UpdateItemAsync(toDoItemModel);
                return Ok("Ok");
            }
            catch
            {
                return Problem("Error during saving the task");
            }
        }


        [Route("GetItem")]
        [HttpGet]
        public async Task<ToDoItemModel> GetItem(int id)
        {
            var item = await _taskService.GetItemAsync(id);
            return item;                      
        }

        [Route("Remind")]
        [HttpGet]
        public List<ReminderModel> Remind()
        {
            List<ReminderModel> model = new List<ReminderModel>();
            string userId = "";
            if(HttpContext.User.Claims.Any())
                userId = HttpContext.User.Claims.First().Value;
            model = _reminderService.GetRemindItems(userId).Select(s => new ReminderModel()
            {
                ItemId = s.Id,
                ItemName = s.Name,
                ToDoListId = s.ToDoListId,
                ListName = _applicationDbContext.ToDoLists.Where(f => f.Id == s.ToDoListId).Select(x=>x.Name).FirstOrDefault()
            }).ToList();
            return model;
        }

       
    }
}
             