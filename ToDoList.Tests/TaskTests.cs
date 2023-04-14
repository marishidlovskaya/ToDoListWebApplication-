using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NUnit.Framework;
using System.Xml.Linq;
using ToDoEntities;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Tests
{
    [TestFixture]
    public class TaskTests
    {
        private static DbContextOptions<ApplicationDbContext> dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "ToDoDatabaseTasks").Options;

        ApplicationDbContext context;
        ITaskRepository taskRepository;


        [OneTimeSetUp]
        public void Setup()
        {
            context = new ApplicationDbContext(dbContextOptions);
            context.Database.EnsureCreated();
            SeedDatabase();
            taskRepository = new TaskRepository(context);
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            context.Database.EnsureDeleted();
        }

        private void SeedDatabase()
        {
            var users = new List<ToDoEntities.User>
            {
              new ToDoEntities.User { Id = 111.ToString(), UserName = "Klare", ShowHiddenLists = false },
              new ToDoEntities.User { Id = 222.ToString(), UserName = "Jon_1987", ShowHiddenLists = false },
              new ToDoEntities.User { Id = 333.ToString(), UserName = "", ShowHiddenLists = true }
            };
            context.Users.AddRange(users);

            var lists = new List<ToDoEntities.ToDoList>
            {
                new ToDoEntities.ToDoList { Id = 10, UserId = 1.ToString(), DateCreated = DateTime.Today, isListHidden = false, Name = "ListOne" },
                new ToDoEntities.ToDoList { Id = 11, UserId = 1.ToString(), DateCreated = DateTime.MinValue, isListHidden = true, Name = "ListTWO!" },
                new ToDoEntities.ToDoList { Id = 12, UserId = 2.ToString(), DateCreated = DateTime.Today, isListHidden = false, Name = "List_3_  " },
                new ToDoEntities.ToDoList { Id = 24, UserId = 2.ToString(), DateCreated = DateTime.MaxValue, isListHidden = false, Name = "" },
                new ToDoEntities.ToDoList { Id = 500, UserId = 2.ToString(), DateCreated = DateTime.Now.AddDays(-15), isListHidden = false, Name = "Reading" }

            };
            context.ToDoLists.AddRange(lists);

            var tasks = new List<ToDoEntities.ToDoItem>
            {
                new ToDoEntities.ToDoItem { Id = 1000, DateCreated = DateTime.Today, DateReminder = DateTime.MaxValue, Description = "Description....", DueDate = null, Name = "Task One", Note = null, Status = "Not Started", ToDoListId = 10 },
                new ToDoEntities.ToDoItem { Id = 1001, DateCreated = DateTime.Today.AddDays(-30), DateReminder = DateTime.Today, Description = "", DueDate = null, Name = "Task2", Note = "notes   ", Status = "In Progress", ToDoListId = 10 },
                new ToDoEntities.ToDoItem { Id = 1002, DateCreated = DateTime.Today, DateReminder = DateTime.Today.AddMinutes(1000000), Description = "50", DueDate = DateTime.MaxValue, Name = "N 1", Note = "my notes....", Status = "Not started", ToDoListId = 11 },
                new ToDoEntities.ToDoItem { Id = 1003, DateCreated = DateTime.Now, DateReminder = DateTime.Now, Description = "50", DueDate = DateTime.MaxValue, Name = "N 2", Note = "notes_yjh&89", Status = "In Progress", ToDoListId = 11 },
                new ToDoEntities.ToDoItem { Id = 1004, DateCreated = DateTime.Parse("01.01.21"), DateReminder = DateTime.Parse("01.01.2050"), Description = "-*----%^7&&()hg", DueDate = null, Name = "Vegetables", Note = "", Status = "Completed", ToDoListId = 24 },
                new ToDoEntities.ToDoItem { Id = 1005, DateCreated = DateTime.Now, DateReminder = DateTime.Today.AddMonths(17), Description = "", DueDate = DateTime.MinValue, Name = "", Note = null, Status = "IN PROGRESS", ToDoListId = 12 },
                new ToDoEntities.ToDoItem { Id = 1006, DateCreated = DateTime.Parse("01.01.1900"), DateReminder = null, Description = null, DueDate = null, Name = "", Note = "", Status = "Completed", ToDoListId = 500 }
            };
            context.ToDoItems.AddRange(tasks);
            context.SaveChanges();
        }

        [Test]
        public async Task GetAllItemsByListIdAsyncTest_AllTasksAreReturnedSuccessfully()
        {
            //Arrange
            int expected = context.ToDoLists.Where(x=>x.Id == 11).SelectMany(f=>f.Items).Count();

            //Act
            var actual = await taskRepository.GetAllItemsByListIdAsync(11);

            //Assert
            Assert.AreEqual(expected, actual.Count());
        }

        [Test]
        public async Task GetAllItemsByListIdAsyncTest_ListDoesNotExistNullIsReturned()
        {
            //Arrange
            var expected = new List<ToDoItemModel>();
            
            //& Act
            var actual = await taskRepository.GetAllItemsByListIdAsync(int.MaxValue);
            
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task GetItemAsyncTest_TaskIsReturnedSuccessfully()
        {
            //Arrange
            var expectedName = context.ToDoItems.Where(x => x.Id == 1002).Select(f => f.Name).FirstOrDefault();

            //& Act
            var actualName = await taskRepository.GetItemAsync(1002);

            //Assert
            Assert.AreEqual(expectedName, actualName.Name);
        }

        [Test]
        public async Task GetItemAsyncTest_ItemDoesNotExistExeptionWasThrown()
        {
            //Arrange & Act
            var actual = await taskRepository.GetItemAsync(int.MaxValue);

            //Assert
            Assert.AreEqual(null, actual);

        }

        [Test]
        public async Task ChangeTaskStatusAsyncTest_StatusIsChangedSuccessfully()
        {
            //Arrange
            var expected = "Completed";

            //Act
            await taskRepository.ChangeTaskStatusAsync(1002, "Completed");
            var actual = context.ToDoItems.Where(x => x.Id == 1002).Select(f => f.Status).FirstOrDefault();

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task ChangeTaskStatusAsyncTest_TaskDoesNotExistExeptionWasThrown()
        {
            Assert.That(() => taskRepository.ChangeTaskStatusAsync(int.MaxValue, "In Progress"), Throws.ArgumentNullException);
        }

        [Test]
        public async Task DeleteTaskByIdAsyncTest_TaskIsDeletedSuccessfully()
        {
            //Arrange & Act
            int numTasks = context.ToDoItems.Count();
            await taskRepository.DeleteTaskByIdAsync(1006);

            //Assert
            Assert.AreEqual(numTasks - 1, context.ToDoItems.Count());
        }

        [Test]
        public async Task DeleteTaskByIdAsyncTest_TaskDoesNotExistExeptionWasThrown()
        {
            Assert.That(() => taskRepository.DeleteTaskByIdAsync(int.MinValue), Throws.ArgumentNullException);
        }


        [Test]
        public async Task AddItemAsyncTest_TaskIsAddedSuccessfully()
        {
            //Arrange & Act
            int numTasks = context.ToDoItems.Count();
            await taskRepository.AddItemAsync(new ToDoItemModel() { Id = 2000, DateCreated = DateTime.Today.AddDays(-10), Name = "TestTask", Status = "In progress", ToDoListId = 11});

            //Assert
            Assert.AreEqual(numTasks + 1, context.ToDoItems.Count());
        }

        [Test]
        public async Task AddItemAsyncTest_TaskNameIsNull_TaskWasNotAdded()
        {
            //Arrange & Act
            int numTasks = context.ToDoItems.Count();
            await taskRepository.AddItemAsync(null);

            //Assert
            Assert.AreEqual(numTasks, context.ToDoItems.Count());
        }

        [Test]
        public async Task UpdateItemAsyncTest_TaskIsUpdatedSuccessfully()
        {
            //Arrange
            var expectedName = "NewName";
            var expectedStatus = "Completed";

            //Act
            await taskRepository.UpdateItemAsync(new ToDoItemModel() { Id = 1005, DateCreated = DateTime.Now, DateReminder = DateTime.Today.AddMonths(17), Description = "", DueDate = DateTime.MinValue, Name = "NewName", Note = null, Status = "Completed", ToDoListId = 12 });
            var actualName = context.ToDoItems.Where(x => x.Id == 1005).Select(f => f.Name).FirstOrDefault();
            var actualStatus = context.ToDoItems.Where(x => x.Id == 1005).Select(f => f.Status).FirstOrDefault();

            //Assert
            Assert.AreEqual(expectedName, actualName);
            Assert.AreEqual(expectedStatus, actualStatus);
        }
    }
}
