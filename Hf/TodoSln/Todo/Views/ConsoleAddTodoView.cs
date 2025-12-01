using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Core;

namespace Todo.Views
{
    internal class ConsoleAddTodoView : IConsoleView
    {
        private readonly ITodoManager _manager;
        private readonly Stack<ConsoleKeyInfo> _enteredKeys;

        public ConsoleAddTodoView(ITodoManager manager)
        {
            _manager = manager;
            _enteredKeys = [];

            WriteHeader();
            Write();
        }

        private void WriteHeader()
        {
            var currentForeground = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Magenta;

            var text = "Adding new Todo";
            Console.SetCursorPosition((ConsoleUI.Width / 2) - (text.Length / 2), 0);
            Console.WriteLine(text);

            Console.ForegroundColor = currentForeground;
        }

        private void Write()
        {
            ConsoleUI.ClearRegion(0, 1, ConsoleUI.Width, ConsoleUI.Height - 2);

            Console.SetCursorPosition(0, 1);

            var currentFgColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkBlue;

            Console.WriteLine("Title;Description;DueDate");

            Console.ForegroundColor = currentFgColor;
        }

        public void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Backspace && _enteredKeys.Count > 0 && _enteredKeys.TryPop(out _))
            {
                Console.Write(" ");
                if (Console.CursorLeft > 0)
                {
                    Console.CursorLeft--;
                }

                return;
            }

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                var text = "";

                while (_enteredKeys.TryPop(out ConsoleKeyInfo info))
                {
                    text = info.KeyChar + text;
                }

                var inputResult = ParseInput(text);

                if (inputResult.Error is not null)
                {
                    Write();

                    var currentFgColor = Console.ForegroundColor;

                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine(inputResult.Error);

                    Console.ForegroundColor = currentFgColor;
                }

                if (inputResult.Success is not null)
                {
                    _manager.Add(inputResult.Success);

                    Write();

                    var currentFgColor = Console.ForegroundColor;

                    Console.ForegroundColor = ConsoleColor.DarkGreen;

                    Console.WriteLine("Todo added successfully");

                    Console.ForegroundColor = currentFgColor;
                }

                return;
            }


            _enteredKeys.Push(keyInfo);
        }

        private Result<TodoItem, string> ParseInput(string input)
        {
            var splits = input.Split(";");

            if (splits.Length != 3)
            {
                return new Result<TodoItem, string>(error: "Malformed input received");
            }

            if (!DateTime.TryParse(splits[2], out DateTime dueDate))
            {
                return new Result<TodoItem, string>(error: "Invalid date received");
            }

            TodoItem item = new() { Title = splits[0], Description = splits[1], DueDate = dueDate };
            return new Result<TodoItem, string>(success: item);
        }
    }
}
