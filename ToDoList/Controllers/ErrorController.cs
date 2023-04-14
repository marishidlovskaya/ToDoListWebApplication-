using Microsoft.AspNetCore.Mvc;

namespace ToDoList.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error()
        {
            return View("Error", "Server error");
        }

        public IActionResult StatusCode(int statusCode)
        {
            if (statusCode == 404)
            {
                return View("Error", "Page not found");
            }
            else
            {
                return View("Error", "An error " + statusCode.ToString() + " occured during processing your request");
            }           
        }
    }
}
