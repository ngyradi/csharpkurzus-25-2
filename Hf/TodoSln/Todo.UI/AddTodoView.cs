using System;
using Todo.Core;

namespace Todo.UI
{
    internal class AddTodoView : ICharacterView
    {
        private readonly ICharacterDisplay _characterDisplay;
        private readonly IViewUtils _viewUtils;
        private readonly ITodoManager _manager;
        private readonly Stack<ConsoleKeyInfo> _enteredKeys;

        public AddTodoView(ITodoManager manager, IViewUtils viewUtils, ICharacterDisplay characterDisplay)
        {
            _manager = manager;
            _viewUtils = viewUtils;
            _characterDisplay = characterDisplay;
            _enteredKeys = [];

            _viewUtils.ClearAndWriteControls();
            _characterDisplay.CursorVisible = true;

            WriteHeader();
            Write();
        }

        private void WriteHeader()
        {
            _viewUtils.WrapWithColors(() =>
            {

                var text = "Adding new Todo";
                _characterDisplay.SetCursorPosition(_characterDisplay.Width / 2 - text.Length / 2, 0);
                _characterDisplay.WriteLine(text);
            }, foregroundColor: ConsoleColor.Magenta);
        }

        private void Write()
        {
            _viewUtils.ClearRegion(0, 1, _characterDisplay.Width, _characterDisplay.Height - 2);

            _characterDisplay.SetCursorPosition(0, 1);

            _viewUtils.WrapWithColors(() =>
            {
                _characterDisplay.WriteLine("Title;Description;DueDate");

            }, foregroundColor: ConsoleColor.DarkBlue);
        }

        public void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Backspace && _enteredKeys.Count > 0 && _enteredKeys.TryPop(out _))
            {
                _characterDisplay.Write(" ");
                if (_characterDisplay.CursorLeft > 0)
                {
                    _characterDisplay.CursorLeft--;
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

                    _viewUtils.WrapWithColors(() =>
                    {
                        _characterDisplay.WriteLine(inputResult.Error);
                    }, foregroundColor: ConsoleColor.Red);
                }

                if (inputResult.Success is not null)
                {
                    _manager.Add(inputResult.Success);

                    Write();

                    _viewUtils.WrapWithColors(() =>
                    {
                        _characterDisplay.WriteLine("Todo added successfully");
                    }, foregroundColor: ConsoleColor.DarkGreen);
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
