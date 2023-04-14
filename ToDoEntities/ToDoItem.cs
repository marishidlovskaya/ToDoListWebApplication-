using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoEntities
{
    public class ToDoItem
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? DateReminder { get; set; }

        public string Status { get; set; }

        public int ToDoListId { get; set; }
        public ToDoList ToDoList { get; set; }

        public string? Note { get; set; }

    }
}
