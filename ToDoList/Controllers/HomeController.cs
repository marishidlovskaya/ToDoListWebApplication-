using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ToDoList.Data;
using ToDoList.Models;
using ToDoList.Services;
using ToDoList.ViewModels;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserService _userService;
        private readonly IToDoListService _toDoListService;

        public HomeController(ApplicationDbContext applicationDbContext, IToDoListService toDoListService)
        {
            _applicationDbContext = applicationDbContext;
            _userService = new UserService(applicationDbContext);
            _toDoListService = toDoListService;
        }


        public async Task<IActionResult> Index()
        {
            ToDoModel _model = new ToDoModel();
            UserModel userModel = new UserModel();
            var user = HttpContext.User.Claims;
            if (!user.Any())
            {
                return View(_model);
            }

            _model.ToDoLists = await _toDoListService.GetAllToDoListsByuserId(user.First().Value);
            _model.ShowHiddenToDoLists = _userService.GetUserInfoById(user.First().Value).ShowHiddenLists;
            return View(_model);
        }

        public async Task<IActionResult> AddNewToDoList(string todolist)
        {
            ToDoListModel toDoModel = JsonSerializer.Deserialize<ToDoListModel>(todolist);
            string userId = HttpContext.User.Claims.First().Value;
            bool isExistedList = _applicationDbContext.ToDoLists.Any(l => l.Id == toDoModel.Id);
            if (isExistedList)
            {
                await _toDoListService.UpdateToDoListByIdAsync(toDoModel.Id, toDoModel.isListHidden, toDoModel.Name);
            }
            else
            {
                await _toDoListService.CreateToDoListAsync(userId, toDoModel.isListHidden, toDoModel.Name);
            }
            return new EmptyResult();
        }

        public async Task<JsonResult> CopyToDoList(int id)
        {
            string userId = HttpContext.User.Claims.First().Value;
            var idNewList = await _toDoListService.CopyToDoListAsync(id);
            return Json(idNewList);
        }

        public async Task<IActionResult> DeleteToDoListById(int id)
        {
            await _toDoListService.DeleteToDoListByIdAsync(id);
            return new EmptyResult();
        }

        public async Task<IActionResult> GetToDoLists()
        {
            ToDoModel _model = new ToDoModel();
            string userId = HttpContext.User.Claims.First().Value;
            _model.ToDoLists = await _toDoListService.GetAllToDoListsByuserId(userId);
            _model.ShowHiddenToDoLists = _userService.GetUserInfoById(userId).ShowHiddenLists;
            return PartialView("_ToDoLists", _model);
        }

        public async Task<JsonResult> isToDoListHidden(int id)
        {
            return Json(await _toDoListService.isToDoListHidden(id));
        }

        public async Task<IActionResult> ChangeToDoListHiddenStatus(bool status)
        {
            string userId = HttpContext.User.Claims.First().Value;
            await _toDoListService.ChangeToDoListHiddenStatus(userId, status);
            return new EmptyResult();
        }



    }
}