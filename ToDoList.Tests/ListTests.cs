using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ToDoEntities;
using ToDoList.Data;



namespace ToDoList.Tests
{
    [TestFixture]
    public class ListTests
    {
        private static DbContextOptions<ApplicationDbContext> dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "ToDoDatabaseLists").Options;

        ApplicationDbContext context;
        IToDoListRepository toDoListRepository;


       [OneTimeSetUp]
        public void Setup()
        {
            context = new ApplicationDbContext(dbContextOptions);
            context.Database.EnsureCreated();
            SeedDatabase();
            toDoListRepository = new ToDoListRepository(context);
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
              new ToDoEntities.User { Id = 1.ToString(), UserName = "User1", ShowHiddenLists = true },
              new ToDoEntities.User { Id = 2.ToString(), UserName = "User2_&&&! !", ShowHiddenLists = false },
              new ToDoEntities.User { Id = 3.ToString(), UserName = "", ShowHiddenLists = true }
            };
            context.Users.AddRange(users);

            var lists = new List<ToDoEntities.ToDoList>
            {
                new ToDoEntities.ToDoList { Id = 1, UserId = 1.ToString(), DateCreated = DateTime.Today, isListHidden = false, Name = "List1" },
                new ToDoEntities.ToDoList { Id = 2, UserId = 1.ToString(), DateCreated = DateTime.MinValue, isListHidden = true, Name = "List2" },
                new ToDoEntities.ToDoList { Id = 3, UserId = 2.ToString(), DateCreated = DateTime.Today, isListHidden = false, Name = "List One" },
                new ToDoEntities.ToDoList { Id = 4, UserId = 2.ToString(), DateCreated = DateTime.MaxValue, isListHidden = false, Name = "List Two&&&!" },
                new ToDoEntities.ToDoList { Id = 5, UserId = 2.ToString(), DateCreated = DateTime.Now.AddDays(-15), isListHidden = false, Name = "ListThree3" }

            };
            context.ToDoLists.AddRange(lists);
            context.SaveChanges();  
        }



        [Test]
        public async Task CreateToDoListAsyncTest_ListIsAddedSuccessfully()
        {
            //Arrange & Act
            int numLists = context.ToDoLists.Count();
            await toDoListRepository.CreateToDoListAsync(int.MaxValue.ToString(), false, "List_1");

            //Assert
            Assert.AreEqual(numLists + 1, context.ToDoLists.Count());
        }

        [Test]
        public async Task CreateToDoListAsyncTest_IIsEdgeCaseListIsAddedSuccessfully()
        {
            //Arrange & Act
            int numLists = context.ToDoLists.Count();
            await toDoListRepository.CreateToDoListAsync(int.MaxValue.ToString(), true, "LIST");

            //Assert
            Assert.AreEqual(numLists + 1, context.ToDoLists.Count());
        }

        [Test]
        public async Task CreateToDoListAsyncTest_ListNameIsNullListWasNotAdded()
        {
            //Arrange & Act
            int numLists = context.ToDoLists.Count();
            await toDoListRepository.CreateToDoListAsync(20.ToString(), false, null);

            //Assert
            Assert.AreEqual(numLists, context.ToDoLists.Count());
        }

        [Test]
        public async Task CopyToDoListAsyncTest_ListIsCopiedSuccessfully()
        {
            //Arrange
            var expected = context.ToDoLists.Where(x => x.Id == 2).Select(f => f.Name).FirstOrDefault();

            //Act
            var copiedListId = await toDoListRepository.CopyToDoListAsync(2);
            var actual = context.ToDoLists.Where(x=>x.Id == copiedListId).Select(f => f.Name).FirstOrDefault();
           
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task CopyToDoListAsyncTest_ListDoesNotExistExeptionWasThrown()
        {
            Assert.That(() => toDoListRepository.CopyToDoListAsync(int.MaxValue), Throws.ArgumentNullException);
        }

        [Test]
        public async Task UpdateToDoListByIdAsyncTest_ListIsUpdatedSuccessfully()
        {
            //Arrange
            var expectedName = "NewName";
            var expectedStatus = true;
            //Act
            await toDoListRepository.UpdateToDoListByIdAsync(4, true, "NewName");
            var actualName = context.ToDoLists.Where(x => x.Id == 4).Select(f=>f.Name).FirstOrDefault();
            var actualStatus = context.ToDoLists.Where(x => x.Id == 4).Select(f => f.isListHidden).FirstOrDefault();

            //Assert
            Assert.AreEqual(expectedName, actualName);
            Assert.AreEqual(expectedStatus, actualStatus);
        }

        [Test]
        public async Task UpdateToDoListByIdAsyncTest_ListDoesNotExistExeptionWasThrown()
        {
            Assert.That(() => toDoListRepository.UpdateToDoListByIdAsync(int.MaxValue, true, "NewName"), Throws.ArgumentNullException);
        }

        [Test]
        public async Task DeleteToDoListByIdAsyncTest_ListIsDeletedSuccessfully()
        {
            //Arrange & Act
            int numLists = context.ToDoLists.Count();
            await toDoListRepository.DeleteToDoListByIdAsync(5);

            //Assert
            Assert.AreEqual(numLists - 1, context.ToDoLists.Count());
        }

        [Test]
        public async Task DeleteToDoListByIdAsyncTest_ListDoesNotExistExeptionWasThrown()
        {
            Assert.That(() => toDoListRepository.DeleteToDoListByIdAsync(int.MinValue), Throws.ArgumentNullException);
        }

        [Test]
        public async Task GetAllToDoListsByuserIdTest_AllListsAreReturned()
        {
            //Arrange & Act
            int expectedResultUser1 = context.ToDoLists.Where(x=>x.UserId == 1.ToString()).Count();
            int expectedResultUser2 = context.ToDoLists.Where(x => x.UserId == 2.ToString()).Count();
            var actuaResultUser1 = await toDoListRepository.GetAllToDoListsByuserId(1.ToString());
            var actuaResultUser2 = await toDoListRepository.GetAllToDoListsByuserId(2.ToString());

            //Assert
            Assert.AreEqual(expectedResultUser1, actuaResultUser1.Count());
            Assert.AreEqual(expectedResultUser2, actuaResultUser2.Count());
        }

        [Test]
        public async Task GetAllTodayToDoListsByuserIdTest_AllTodayListsAreReturned()
        {
            //Arrange & Act
            var listsUser1 = context.ToDoLists.Where(x => x.UserId == 1.ToString());
            var expectedResultUser1 = listsUser1.SelectMany(x => x.Items).Where(f => f.DueDate == DateTime.Today);
            var listsUser2 = context.ToDoLists.Where(x => x.UserId == 2.ToString());
            var expectedResultUser2 = listsUser2.SelectMany(x => x.Items).Where(f => f.DueDate == DateTime.Today);
            var actuaResultUser1 = await toDoListRepository.GetAllTodayToDoListsByuserId(1.ToString());
            var actuaResultUser2 = await toDoListRepository.GetAllTodayToDoListsByuserId(2.ToString());

            //Assert
            Assert.AreEqual(expectedResultUser1.Count(), actuaResultUser1.Count());
            Assert.AreEqual(expectedResultUser2.Count(), actuaResultUser2.Count());
        }

        [Test]
        public async Task GetToDoListByListIdTest_ListIsReturned()
        {
            //Arrange
            var expectedName = context.ToDoLists.Where(x => x.Id == 4).Select(f=>f.Name).FirstOrDefault();

            //Act
            var actual = await toDoListRepository.GetToDoListByListId(4);

             //Assert
            Assert.AreEqual(expectedName, actual.Name);
        }

        [Test]
        public async Task GetToDoListByListIdTest_ListDoesNotExistNullIsReturned()
        {
            //Arrange & Act
            var actual = await toDoListRepository.GetToDoListByListId(int.MaxValue);

            //Assert
            Assert.AreEqual(null, actual);
        }

        [Test]
        public async Task ChangeToDoListHiddenStatusTest_StatusIsChangedSuccessfully()
        {
            //Arrange
            var expected = true;

            //Act
            await toDoListRepository.ChangeToDoListHiddenStatus(2.ToString(), true);
            var actual = context.Users.Where(x => x.Id == 2.ToString()).Select(f => f.ShowHiddenLists).FirstOrDefault();

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task ChangeToDoListHiddenStatusTest_UserDoesNotExistExeptionWasThrown()
        {
            Assert.That(() => toDoListRepository.ChangeToDoListHiddenStatus(int.MaxValue.ToString(), true), Throws.ArgumentNullException);
        }


    }
}