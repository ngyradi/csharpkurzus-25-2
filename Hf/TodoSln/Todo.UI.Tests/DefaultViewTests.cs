using Moq;
using Todo.Core;
using Microsoft.Extensions.Time.Testing;

namespace Todo.UI.Tests
{
    internal class DefaultViewTests
    {
        Mock<ITodoManager> todoManager;
        Mock<ICharacterDisplay> characterDisplay;
        Mock<IViewUtils> viewUtils;

        [SetUp]
        public void Setup()
        {
            todoManager = new Mock<ITodoManager>();
            characterDisplay = new Mock<ICharacterDisplay>();
            viewUtils = new Mock<IViewUtils>();
            viewUtils.Setup(m => m.WrapWithColors(It.IsAny<Action>(), It.IsAny<ConsoleColor?>(), It.IsAny<ConsoleColor?>())).Callback<Action, ConsoleColor?, ConsoleColor?>((action, fg, bg) =>
            {
                action();
            });
        }

        [Test]
        public void Constructor_ShouldWriteHeader()
        {
            var view = CreateView();

            characterDisplay.Verify(m => m.WriteLine("Overview"), Times.Once);
        }

        [Test]
        public void Constructor_ShouldWriteUpcomingTodos_WhenAtleastOneExists()
        {
            var upcomingTodo = new TodoItem() { Title = "Upcoming", Description = "Description", IsDone = false, DueDate = new DateTime(2020, 10, 10) };
            var pastTodo = new TodoItem() { Title = "Todo from the past", Description = "Description", IsDone = false, DueDate = new DateTime(2020, 05, 10) };

            var list = new List<TodoItem>
            {
                upcomingTodo,
                pastTodo
            };

            todoManager.Setup(m => m.GetTodoItems()).Returns(() => list);

            var timeProvider = new FakeTimeProvider(new DateTimeOffset(2020, 10, 09, 0, 0, 0, TimeSpan.Zero));

            var view = CreateView(timeProvider);

            viewUtils.Verify(m => m.WriteTodo(upcomingTodo, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            viewUtils.Verify(m => m.WriteTodo(pastTodo, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void Constructor_ShouldWriteUpcomingTodos_WhenNoTodoExists()
        {
            var list = new List<TodoItem>
            {
            };

            todoManager.Setup(m => m.GetTodoItems()).Returns(() => list);

            var timeProvider = new FakeTimeProvider(new DateTimeOffset(2020, 10, 09, 0, 0, 0, TimeSpan.Zero));

            var view = CreateView(timeProvider);

            characterDisplay.Verify(m => m.WriteLine("No upcoming todos"), Times.Once);
        }

        [Test]
        public void Constructor_ShouldWriteLastCompletedByMonthTodosInTheCorrectFormat()
        {
            var todo1 = new TodoItem() { 
                Title = "Todo from October", 
                Description = "Description", 
                IsDone = true, 
                DueDate = new DateTime(2020, 10, 10)
            };
            var todo2 = new TodoItem()
            {
                Title = "Todo from March",
                Description = "Description",
                IsDone = true,
                DueDate = new DateTime(2020, 03, 10)
            };
            var todo3 = new TodoItem()
            {
                Title = "Incomplete Todo from March",
                Description = "Description",
                IsDone = false,
                DueDate = new DateTime(2020, 03, 10)
            };

            var list = new List<TodoItem> { todo1, todo2, todo3, };

            todoManager.Setup(m => m.GetTodoItems()).Returns(() => list);
            var timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 10, 09, 0, 0, 0, TimeSpan.Zero));

            var view = CreateView(timeProvider);

            characterDisplay.Verify(m => m.Write($"{new DateTime(2020, 10, 1):yyyy MMMM}"), Times.Once);
            viewUtils.Verify(m => m.WriteTodo(todo1, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            characterDisplay.Verify(m => m.Write($"{new DateTime(2020, 03, 1):yyyy MMMM}"), Times.Once);
            viewUtils.Verify(m => m.WriteTodo(todo2, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            viewUtils.Verify(m => m.WriteTodo(todo3, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        private DefaultView CreateView(TimeProvider? timeProvider = null)
        {
            return new DefaultView(todoManager.Object, viewUtils.Object, characterDisplay.Object, timeProvider is null ? new FakeTimeProvider() : timeProvider);
        }
    }
}
