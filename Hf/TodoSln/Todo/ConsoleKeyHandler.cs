using Todo.Core;

namespace Todo
{
    internal class ConsoleKeyHandler(ITodoManager manager) : IConsoleKeyHandler
    {
        private Queue<ConsoleKeyInfo> _enteredKeys = [];

        public bool Handle(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Q && keyInfo.Modifiers == ConsoleModifiers.Control)
            {
                return false;
            }

            if (keyInfo.Key == ConsoleKey.A && keyInfo.Modifiers == ConsoleModifiers.Control && ConsoleUI.InputMode != InputMode.Adding)
            {
                ConsoleUI.InputMode = InputMode.Adding;
                ConsoleUI.Clear();

                return true;
            }

            if (keyInfo.Key == ConsoleKey.X && keyInfo.Modifiers == ConsoleModifiers.Control && ConsoleUI.InputMode != InputMode.None)
            {
                ConsoleUI.InputMode = InputMode.None;
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

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                // process the entered keys based on the input mode
                var text = "";

                while (_enteredKeys.TryDequeue(out ConsoleKeyInfo info))
                {
                    text += info.KeyChar;
                }

                try
                {
                    HandleEnter(text);
                    ConsoleUI.InputMode = InputMode.None;
                    ConsoleUI.Clear();
                }
                catch (Exception ex)
                {
                    ConsoleUI.InputMode = InputMode.None;
                    ConsoleUI.Clear();
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                _enteredKeys.Enqueue(keyInfo);
            }

            return true;
        }

        private void HandleEnter(string text)
        {
                if (ConsoleUI.InputMode == InputMode.Adding)
                {
                    var asd = new AddTodoConsoleAction(manager);
                    asd.Execute(text);
                }
                else if (ConsoleUI.InputMode == InputMode.Saving)
                {
                    var asd = new SaveChangesConsoleAction(manager);
                    asd.Execute(text);
                }
        }
    }
}