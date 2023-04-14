using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ToDoEntities
{
    public class User
    {
        [Key]
        public string Id { get; set; }

        public string UserName { get; set; }

        public bool ShowHiddenLists { get; set; }

        public ICollection<ToDoList> ToDoLists { get; set; }
    }
}
