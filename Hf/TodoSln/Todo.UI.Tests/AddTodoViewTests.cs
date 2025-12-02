using Moq;
using Todo.Core;

namespace Todo.UI.Tests
{
    internal class AddTodoViewTests
    {
        AddTodoView view;

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

            view = new AddTodoView(todoManager.Object, viewUtils.Object, characterDisplay.Object);
        }

        [Test]
        public void Constructor_ShouldClearAndWriteControls()
        {
            viewUtils.Verify(m => m.ClearAndWriteControls(), Times.Once);
        }

        [Test]
        public void Constructor_ShouldSetCursorToBeVisible()
        {
            characterDisplay.VerifySet(m => m.CursorVisible = true);
        }

        [Test]
        public void Constructor_ShouldWriteHeader()
        {
            characterDisplay.Verify(m => m.WriteLine("Adding new Todo"), Times.Once);
        }

        [Test]
        public void Constructor_ShouldWriteTodoFormat()
        {
            characterDisplay.Verify(m => m.WriteLine("Title;Description;DueDate"), Times.Once);
        }

        [Test]
        public void HandleKey_ShouldClearOneCharacter_OnBackspace()
        {
            view.HandleKey(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));

            view.HandleKey(new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false));

            characterDisplay.Verify(m => m.Write(" "), Times.Once);
        }

        [Test]
        public void HandleKey_ShouldAddNewTodo_WithValidInput()
        {
            view.HandleKey(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
            view.HandleKey(new ConsoleKeyInfo(';', ConsoleKey.OemComma, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
            view.HandleKey(new ConsoleKeyInfo(';', ConsoleKey.OemComma, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('2', ConsoleKey.D2, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('0', ConsoleKey.D0, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('0', ConsoleKey.D0, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('5', ConsoleKey.D5, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('.', ConsoleKey.OemPeriod, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('1', ConsoleKey.D1, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('.', ConsoleKey.OemPeriod, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('1', ConsoleKey.D1, false, false, false));

            view.HandleKey(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));

            todoManager.Verify(m => m.Add(It.IsAny<TodoItem>()), Times.Once);
        }

        [Test]
        public void HandleKey_ShouldShowError_WithMalformedInput()
        {
            view.HandleKey(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));

            todoManager.Verify(m => m.Add(It.IsAny<TodoItem>()), Times.Never);
            characterDisplay.Verify(m => m.WriteLine("Malformed input received"), Times.AtLeastOnce);
        }

        [Test]
        public void HandleKey_ShouldShowError_WithInvalidDate()
        {
            view.HandleKey(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
            view.HandleKey(new ConsoleKeyInfo(';', ConsoleKey.OemComma, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
            view.HandleKey(new ConsoleKeyInfo(';', ConsoleKey.OemComma, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('2', ConsoleKey.D2, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('0', ConsoleKey.D0, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('0', ConsoleKey.D0, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('5', ConsoleKey.D5, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('.', ConsoleKey.OemPeriod, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('1', ConsoleKey.D1, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('.', ConsoleKey.OemPeriod, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('3', ConsoleKey.D3, false, false, false));
            view.HandleKey(new ConsoleKeyInfo('3', ConsoleKey.D3, false, false, false));

            view.HandleKey(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));

            todoManager.Verify(m => m.Add(It.IsAny<TodoItem>()), Times.Never);
            characterDisplay.Verify(m => m.WriteLine("Invalid date received"), Times.AtLeastOnce);
        }
    }
}
