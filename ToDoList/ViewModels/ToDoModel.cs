using ToDoList.Models;

namespace ToDoList.ViewModels
{
    public class ToDoModel
    {
        public List<ToDoListModel> ToDoLists { get; set; }
        public bool ShowHiddenToDoLists { get; set; }
    }
}
