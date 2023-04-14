using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ToDoList.Data;
using ToDoList.Models;
using ToDoList.Services;
using ToDoList.ViewModels;

namespace ToDoList.Controllers
{
    public class ToDoController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IToDoListService _toDoListService;
        private readonly ITaskService _taskService;

        public ToDoController(ApplicationDbContext applicationDbContext, IToDoListService toDoListService, ITaskService taskService)
        {
            _applicationDbContext = applicationDbContext;
            _toDoListService = toDoListService;
            _taskService = taskService;
        }
        public async Task<IActionResult> Index(int id)
        {        
            ToDoListModel _model = new ToDoListModel();
            _model = await _toDoListService.GetToDoListByListId(id);
             return View(_model);     
        }

        public async Task<IActionResult> GetListsDueToday()
        {
            string userId = HttpContext.User.Claims.First().Value;
            List<ToDoListModel> model = new List<ToDoListModel>();
            model = await _toDoListService.GetAllTodayToDoListsByuserId(userId);
            return View("~/Views/ToDo/AllToDoLists.cshtml", model);

        }

        public async Task<IActionResult> GetPlannedLists()
        {
            string userId = HttpContext.User.Claims.First().Value;
            List<ToDoListModel> model = new List<ToDoListModel>();
            model = await _toDoListService.GetAllToDoListsByuserId(userId);
            return View("~/Views/ToDo/AllToDoLists.cshtml", model);
        }
    
        public async Task<JsonResult> ChangeTaskStatus(int taskId, string status)
        {
            await _taskService.ChangeTaskStatusAsync(taskId, status);

            return Json("");
        }

        public async Task<IActionResult> DeleteTaskById(int taskId)
        {
            await _taskService.DeleteTaskByIdAsync(taskId);
            return new EmptyResult();
        }

        public async Task<IActionResult> GetToDoItems()
        {
            int id = int.Parse(HttpContext.Request.Query["id"].ToString());
            List<ToDoItemModel> _model = new List<ToDoItemModel>();
            _model = await _taskService.GetAllItemsByListIdAsync(id);

            return PartialView("_ToDoItems", _model);
        }
    }
}
