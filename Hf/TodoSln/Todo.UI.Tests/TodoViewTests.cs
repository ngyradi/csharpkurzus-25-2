using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Core;

namespace Todo.UI.Tests
{
    internal class TodoViewTests
    {
        TodoView view;

        Mock<ITodoManager> todoManager;
        Mock<ICharacterDisplay> characterDisplay;
        Mock<IViewUtils> viewUtils;

        TodoItem pendingTodo1;
        TodoItem pendingTodo2;
        TodoItem doneTodo1;
        TodoItem doneTodo2;

        [SetUp]
        public void Setup()
        {
            pendingTodo1 = new TodoItem() { Title = "pending 1", Description = "desc", IsDone = false, DueDate = new DateTime(2025, 12, 12) };
            pendingTodo2 = new TodoItem() { Title = "pending 2", Description = "desc", IsDone = false, DueDate = new DateTime(2025, 12, 13) };
            doneTodo1 = new TodoItem() { Title = "done 1", Description = "desc", IsDone = true, DueDate = new DateTime(2025, 12, 04) };
            doneTodo2 = new TodoItem() { Title = "done 2", Description = "desc", IsDone = true, DueDate = new DateTime(2025, 12, 07) };

            var list = new List<TodoItem>
            {
                pendingTodo1,
                pendingTodo2,
                doneTodo1,
                doneTodo2
            };

            todoManager = new Mock<ITodoManager>();
            todoManager.Setup(m => m.GetTodoItems()).Returns(list);

            characterDisplay = new Mock<ICharacterDisplay>();
            characterDisplay.Setup(m => m.Width).Returns(100);
            characterDisplay.Setup(m => m.Height).Returns(50);

            viewUtils = new Mock<IViewUtils>();
            viewUtils.Setup(m => m.WrapWithColors(It.IsAny<Action>(), It.IsAny<ConsoleColor?>(), It.IsAny<ConsoleColor?>())).Callback<Action, ConsoleColor?, ConsoleColor?>((action, fg, bg) =>
            {
                action();
            });

            view = new TodoView(todoManager.Object, viewUtils.Object, characterDisplay.Object);
        }

        [Test]
        public void Constructor_ShouldWriteHeader()
        {
            characterDisplay.Verify(m => m.WriteLine("Viewing todos"));
        }

        [Test]
        public void Constructor_ShouldWritePendingAndDoneTodoColumns()
        {
            viewUtils.Verify(m => m.WriteCenteredText("Pending", It.IsAny<int>()));
            viewUtils.Verify(m => m.WriteTodo(pendingTodo2, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));
            viewUtils.Verify(m => m.WriteTodo(pendingTodo1, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));

            viewUtils.Verify(m => m.WriteCenteredText("Done", It.IsAny<int>()));
            viewUtils.Verify(m => m.WriteTodo(doneTodo2, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));
            viewUtils.Verify(m => m.WriteTodo(doneTodo1, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));
        }

        [Test]
        public void HandleKey_ShouldShowDoneColumnAsSelected_OnRightArrow()
        {
            view.HandleKey(new ConsoleKeyInfo(' ', ConsoleKey.RightArrow, false, false, false));

            viewUtils.Verify(m => m.WrapWithColors(It.IsAny<Action>(), ConsoleColor.Green, ConsoleColor.White));
            viewUtils.Verify(m => m.WriteCenteredText("Done", It.IsAny<int>()));
            viewUtils.Verify(m => m.WriteTodo(doneTodo2, true, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));
        }

        [Test]
        public void HandleKey_ShouldShowPendingColumnAsSelected_OnRightArrowWhenDoneColumnWasSelected()
        {
            view.HandleKey(new ConsoleKeyInfo(' ', ConsoleKey.RightArrow, false, false, false));
            view.HandleKey(new ConsoleKeyInfo(' ', ConsoleKey.LeftArrow, false, false, false));

            viewUtils.Verify(m => m.WrapWithColors(It.IsAny<Action>(), ConsoleColor.Red, ConsoleColor.White));
            viewUtils.Verify(m => m.WriteCenteredText("Pending", It.IsAny<int>()), Times.Exactly(3));
            viewUtils.Verify(m => m.WriteTodo(pendingTodo2, true, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));
        }

        [Test]
        public void HandleKey_ShouldScrollPendingColumnDown_OnDownArrow()
        {
            view.HandleKey(new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, false, false, false));

            viewUtils.Verify(m => m.WriteTodo(pendingTodo1, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
            viewUtils.Verify(m => m.WriteTodo(pendingTodo1, true, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            viewUtils.Verify(m => m.WriteTodo(pendingTodo2, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void HandleKey_ShouldScrollPendingColumnUp_OnUpArrow()
        {
            view.HandleKey(new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, false, false, false));
            view.HandleKey(new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, false, false, false));

            viewUtils.Verify(m => m.WriteTodo(pendingTodo1, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(3));
            viewUtils.Verify(m => m.WriteTodo(pendingTodo1, true, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            viewUtils.Verify(m => m.WriteTodo(pendingTodo2, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
            viewUtils.Verify(m => m.WriteTodo(pendingTodo2, true, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        }

        [Test]
        public void HandleKeyShouldScrollDoneColumnDown_OnDownArrowWhenDoneColumnIsSelected()
        {
            view.HandleKey(new ConsoleKeyInfo(' ', ConsoleKey.RightArrow, false, false, false));
            view.HandleKey(new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, false, false, false));

            viewUtils.Verify(m => m.WriteTodo(doneTodo1, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(3));
            viewUtils.Verify(m => m.WriteTodo(doneTodo1, true, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            viewUtils.Verify(m => m.WriteTodo(doneTodo2, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        }

        [Test]
        public void HandleKeyShouldScrollDoneColumnUp_OnUpArrowWhenDoneColumnIsSelected()
        {
            view.HandleKey(new ConsoleKeyInfo(' ', ConsoleKey.RightArrow, false, false, false));
            view.HandleKey(new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, false, false, false));
            view.HandleKey(new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, false, false, false));

            viewUtils.Verify(m => m.WriteTodo(doneTodo1, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(4));
            viewUtils.Verify(m => m.WriteTodo(doneTodo1, true, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            viewUtils.Verify(m => m.WriteTodo(doneTodo2, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(3));
            viewUtils.Verify(m => m.WriteTodo(doneTodo2, true, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        }

        [Test]
        public void HandleKey_ShouldSetPendingTodoToDone_OnEnter()
        {
            view.HandleKey(new ConsoleKeyInfo(' ', ConsoleKey.Enter, false, false, false));

            todoManager.Verify(m => m.Update(pendingTodo2, It.Is<TodoItem>((todo) => todo.IsDone == true && todo.Title.Equals("pending 2"))));
        }

        [Test]
        public void HandleKey_ShouldSetDoneTodoToPending_OnEnter()
        {
            view.HandleKey(new ConsoleKeyInfo(' ', ConsoleKey.RightArrow, false, false, false));
            view.HandleKey(new ConsoleKeyInfo(' ', ConsoleKey.Enter, false, false, false));

            todoManager.Verify(m => m.Update(doneTodo2, It.Is<TodoItem>((todo) => todo.IsDone == false && todo.Title.Equals("done 2"))));
        }
    }
}
