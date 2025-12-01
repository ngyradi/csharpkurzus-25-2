using Todo.Core;

namespace Todo.Views
{
    internal class ConsoleSaveView: IConsoleView
    {
        private readonly ITodoManager _manager;

        public ConsoleSaveView(ITodoManager manager)
        {
            _manager = manager;

            WriteHeader();
            Write();
        }

        public void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key != ConsoleKey.Y)
            {
                Write();
                return;
            }

            Console.CursorLeft--;
            Console.Write(' ');

            Write();

            var result = _manager.Save();

            if (result.Success is not null)
            {
                ConsoleUI.WrapWithColors(() =>
                {
                    Console.WriteLine(result.Success);

                }, foregroundColor: ConsoleColor.DarkGreen);
            }else if (result.Error is not null)
            {
                ConsoleUI.WrapWithColors(() =>
                {
                    Console.WriteLine(result.Error);
                }, foregroundColor: ConsoleColor.Red);
            }
        }

        private void Write()
        {
            ConsoleUI.ClearRegion(0, 1, ConsoleUI.Width, 3);
            Console.SetCursorPosition(0, 1);
            Console.WriteLine("Type 'y' to save");
        }

        private void WriteHeader()
        {
            ConsoleUI.WrapWithColors(() =>
            {
                var text = "Are you sure you want to save the changes?";
                Console.SetCursorPosition((ConsoleUI.Width / 2) - (text.Length / 2), 0);
                Console.WriteLine(text);
            }, foregroundColor: ConsoleColor.Magenta);
        }
    }
}
