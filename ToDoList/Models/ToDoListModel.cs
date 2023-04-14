using ToDoEntities;

namespace ToDoList.Models
{
    public class ToDoListModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime DateCreated { get; set; }

        public string UserId { get; set; }

        public bool isListHidden { get; set; }

        public List<ToDoItemModel> toDoItems { get; set; } 

    }
}
