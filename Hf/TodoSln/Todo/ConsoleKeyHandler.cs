using System.Diagnostics;
using Todo.Core;
using Todo.Views;

namespace Todo
{
    internal class ConsoleKeyHandler : IConsoleKeyHandler
    {
        private readonly ITodoManager _manager;
        private IConsoleView? _view = null;

        private readonly Stack<ConsoleKeyInfo> _enteredKeys;


        public ConsoleKeyHandler(ITodoManager manager)
        {
            _manager = manager;
            _enteredKeys = [];

            SwitchInputMode(InputMode.None);
        }

        public bool Handle(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.C && keyInfo.Modifiers == ConsoleModifiers.Control)
            {
                return false;
            }

            if (keyInfo.Key == ConsoleKey.A && keyInfo.Modifiers == ConsoleModifiers.Control)
            {
                SwitchInputMode(InputMode.Adding);

                return true;
            }

            if (keyInfo.Key == ConsoleKey.X && keyInfo.Modifiers == ConsoleModifiers.Control)
            {
                SwitchInputMode(InputMode.None);

                return true;
            }

            if (keyInfo.Key == ConsoleKey.W && keyInfo.Modifiers == ConsoleModifiers.Control)
            {
                SwitchInputMode(InputMode.Listing);

                return true;
            }

            if (keyInfo.Key == ConsoleKey.K && keyInfo.Modifiers == ConsoleModifiers.Control)
            {
                SwitchInputMode(InputMode.Saving);

                Console.WriteLine("Type 'y' to save");

                return true;
            }

            if (keyInfo.Key == ConsoleKey.Backspace && _enteredKeys.Count > 0 && _enteredKeys.TryPop(out _))
            {
                Console.Write(" ");
                if (Console.CursorLeft > 0)
                {
                    Console.CursorLeft--;
                }

                return true;
            }

            _view?.HandleKey(keyInfo);

            if (_view is null && keyInfo.Key == ConsoleKey.Enter)
            {
                var text = "";

                while (_enteredKeys.TryPop(out ConsoleKeyInfo info))
                {
                    text = info.KeyChar + text;
                }

                try
                {
                    var result = HandleEnter(text);

                    if (result != null)
                    {
                        if (result.Success is not null)
                        {
                            ConsoleUI.InputMode = InputMode.None;
                            ConsoleUI.Clear();

                            var currentColor = Console.ForegroundColor;

                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine(result.Success);

                            Console.ForegroundColor = currentColor;
                        }
                        else if (result.Error is not null)
                        {
                            ConsoleUI.Clear();

                            var currentColor = Console.ForegroundColor;

                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine(result.Error);

                            Console.ForegroundColor = currentColor;
                        }
                    }
                    else
                    {
                        ConsoleUI.Clear();
                    }
                }
                catch (Exception ex)
                {
                    ConsoleUI.Clear();
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                _enteredKeys.Push(keyInfo);
            }

            return true;
        }

        private void SwitchInputMode(InputMode inputMode)
        {
            ConsoleUI.InputMode = inputMode;
            ConsoleUI.Clear();

            if (inputMode == InputMode.Listing)
            {
                Console.CursorVisible = false;
                _view = new ConsoleTodoView(_manager);
            }
            else if (inputMode == InputMode.None)
            {
                Console.CursorVisible = false;
                _view = new ConsoleDefaultView(_manager);
            }
            else
            {
                Console.CursorVisible = true;
                _view = null;
            }

            _enteredKeys.Clear();
        }

        private Result<string, string>? HandleEnter(string text)
        {
            Debug.WriteLine($"{ConsoleUI.InputMode} - {text}");

            if (ConsoleUI.InputMode == InputMode.Adding)
            {
                var addTodoConsoleAction = new AddTodoConsoleAction(_manager);
                return addTodoConsoleAction.Execute(text);
            }
            else if (ConsoleUI.InputMode == InputMode.Saving)
            {
                var saveChangesConsoleAction = new SaveChangesConsoleAction(_manager);
                return saveChangesConsoleAction.Execute(text);
            }

            return null;
        }
    }
}