using System.Diagnostics;
using Todo.Core;
using Todo.Views;

namespace Todo
{
    internal class ConsoleKeyHandler : IConsoleKeyHandler
    {
        private readonly ITodoManager _manager;
        private IConsoleView? _view = null;

        public ConsoleKeyHandler(ITodoManager manager)
        {
            _manager = manager;

            SwitchInputMode(InputMode.None);
        }

        public bool Handle(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.C && keyInfo.Modifiers == ConsoleModifiers.Control)
            {
                return false;
            }

            if (keyInfo.Modifiers == ConsoleModifiers.Control)
            {
                HandleControlKey(keyInfo);
            }

            _view?.HandleKey(keyInfo);

            return true;
        }

        private void HandleControlKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.A)
            {
                SwitchInputMode(InputMode.Adding);
            }

            if (keyInfo.Key == ConsoleKey.X)
            {
                SwitchInputMode(InputMode.None);
            }

            if (keyInfo.Key == ConsoleKey.W)
            {
                SwitchInputMode(InputMode.Listing);
            }

            if (keyInfo.Key == ConsoleKey.K)
            {
                SwitchInputMode(InputMode.Saving);
            }
        }

        private void SwitchInputMode(InputMode inputMode)
        {
            ConsoleUI.Clear();

            Console.CursorVisible = true;

            _view = inputMode switch
            {
                InputMode.None => new ConsoleDefaultView(_manager),
                InputMode.Adding => new ConsoleAddTodoView(_manager),
                InputMode.Saving => new ConsoleSaveView(_manager),
                InputMode.Listing => new ConsoleTodoView(_manager),
                _ => throw new InvalidOperationException("Branch for set mode does not exist"),
            };
        }
    }
}