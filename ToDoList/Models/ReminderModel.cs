namespace ToDoList.Models
{
    public class ReminderModel
    {
        public int ItemId { get; set; }
        public int ToDoListId { get; set; }  
        public string ListName  { get; set; }

        public string ItemName { get; set; }
    }
}
