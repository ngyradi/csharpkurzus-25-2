using Moq;
using Todo.Core;

namespace Todo.UI.Tests
{
    internal class SaveViewTests
    {
        SaveView view;

        Mock<ITodoManager> todoManager;
        Mock<IViewUtils> viewUtils;
        Mock<ICharacterDisplay> characterDisplay;

        [SetUp]
        public void Setup()
        {
            todoManager = new Mock<ITodoManager>();
            todoManager.Setup(m => m.Save()).Returns(new Result<string, string>(success: "success message"));

            characterDisplay = new Mock<ICharacterDisplay>();
            viewUtils = new Mock<IViewUtils>();
            viewUtils.Setup(m => m.WrapWithColors(It.IsAny<Action>(), It.IsAny<ConsoleColor?>(), It.IsAny<ConsoleColor?>())).Callback<Action, ConsoleColor?, ConsoleColor?>((action, fg, bg) =>
            {
                action();
            });

            view = new SaveView(todoManager.Object, viewUtils.Object, characterDisplay.Object);
        }

        [Test]
        public void Constructor_ShouldWriteHeader()
        {
            characterDisplay.Verify(m => m.WriteLine("Are you sure you want to save the changes?"), Times.Once);
        }

        [Test]
        public void Constructor_ShouldWriteConfirmationMessage()
        {
            characterDisplay.Verify(m => m.WriteLine("Type 'y' to save"), Times.Once);
        }

        [Test]
        public void HandleKey_ShouldWriteConfirmationMessage_WhenYWasNotEntered()
        {
            view.HandleKey(new ConsoleKeyInfo('n', ConsoleKey.N, false, false, false));

            characterDisplay.Verify(m => m.WriteLine("Type 'y' to save"), Times.Exactly(2));
        }

        [Test]
        public void HandleKey_ShouldSave_WhenYWasEntered()
        {
            view.HandleKey(new ConsoleKeyInfo('y', ConsoleKey.Y, false, false, false));

            todoManager.Verify(m => m.Save(), Times.Once);
        }

        [Test]
        public void HandleKey_ShouldDisplaySuccessMessage_WhenYWasEnteredAndSaveWasSuccessful()
        {
            view.HandleKey(new ConsoleKeyInfo('y', ConsoleKey.Y, false, false, false));

            characterDisplay.Verify(m => m.WriteLine("success message"));
        }

        [Test]
        public void HandleKey_ShouldDisplayErrorMessage_WhenYWasEnteredAndSaveWasUnsuccessful()
        {
            todoManager.Setup(m => m.Save()).Returns(new Result<string, string>(error: "error message"));
            view.HandleKey(new ConsoleKeyInfo('y', ConsoleKey.Y, false, false, false));

            characterDisplay.Verify(m => m.WriteLine("error message"));
        }
    }
}
