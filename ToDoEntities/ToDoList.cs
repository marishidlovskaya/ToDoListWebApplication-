using System.ComponentModel.DataAnnotations;

namespace ToDoEntities
{
    public class ToDoList
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public DateTime DateCreated { get; set; }

        public bool isListHidden { get; set; }
        public ICollection<ToDoItem> Items { get; set; }
    }
}