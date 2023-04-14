using ToDoEntities;

namespace ToDoList.Models
{
    public class ToDoItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? DateReminder { get; set; }

        public string Status { get; set; }

        bool isReminder { get; set; }

        public string? Note { get; set; }

        public int ToDoListId { get; set; }
    }
}
