using ToDoEntities;

namespace ToDoList.Models
{
    public class UserModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public bool ShowHiddenLists { get; set; }

        public List<ToDoEntities.ToDoList> ToDoLists { get; set; }
    }
}
