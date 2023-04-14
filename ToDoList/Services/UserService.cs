using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public UserService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public UserModel GetUserInfoById(string userId)
        {
            var user = _applicationDbContext.Users.Where(x => x.Id == userId).FirstOrDefault();
            UserModel model = new UserModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                ShowHiddenLists = user.ShowHiddenLists,
                ToDoLists = _applicationDbContext.ToDoLists.Where(x => x.UserId == user.Id).ToList()
            };




            return model;
        }
    }
}
