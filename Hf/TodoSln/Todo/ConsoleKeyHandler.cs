using System.Diagnostics;
using Todo.Core;
using static System.Net.Mime.MediaTypeNames;

namespace Todo
{
    internal class ConsoleKeyHandler(ITodoManager manager) : IConsoleKeyHandler
    {
        private Stack<ConsoleKeyInfo> _enteredKeys = [];

        public bool Handle(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.C && keyInfo.Modifiers == ConsoleModifiers.Control)
            {
                return false;
            }

            if (keyInfo.Key == ConsoleKey.A && keyInfo.Modifiers == ConsoleModifiers.Control && ConsoleUI.InputMode != InputMode.Adding)
            {
                ConsoleUI.InputMode = InputMode.Adding;
                ConsoleUI.Clear();

                return true;
            }

            if (keyInfo.Key == ConsoleKey.X && keyInfo.Modifiers == ConsoleModifiers.Control && ConsoleUI.InputMode != InputMode.List)
            {
                ConsoleUI.InputMode = InputMode.List;
                ConsoleUI.Clear();

                _enteredKeys.Clear();

                return true;
            }

            if (keyInfo.Key == ConsoleKey.K && keyInfo.Modifiers == ConsoleModifiers.Control && ConsoleUI.InputMode != InputMode.Saving)
            {
                ConsoleUI.InputMode = InputMode.Saving;
                ConsoleUI.Clear();

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
                
            if (keyInfo.Key == ConsoleKey.Enter)
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
                        if (result.Success is not null){
                            ConsoleUI.InputMode = InputMode.List;
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

        private Result<string, string>? HandleEnter(string text)
        {
            Debug.WriteLine($"{ConsoleUI.InputMode} - {text}");

            if (ConsoleUI.InputMode == InputMode.Adding)
            {
                var addTodoConsoleAction = new AddTodoConsoleAction(manager);
                return addTodoConsoleAction.Execute(text);
            }
            else if (ConsoleUI.InputMode == InputMode.Saving)
            {
                var saveChangesConsoleAction = new SaveChangesConsoleAction(manager);
                return saveChangesConsoleAction.Execute(text);
            }

            return null;
        }
    }
}