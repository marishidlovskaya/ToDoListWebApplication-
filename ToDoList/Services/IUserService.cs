using ToDoList.Models;

namespace ToDoList.Services
{
    public interface IUserService
    {
        public UserModel GetUserInfoById(string userId);
    }
}
