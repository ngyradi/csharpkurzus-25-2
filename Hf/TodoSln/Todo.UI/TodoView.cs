using Todo.Core;

namespace Todo.UI
{
    internal class TodoView : ICharacterView
    {
        private const char SeparatorCharacter = '┃';

        private readonly ICharacterDisplay _characterDisplay;
        private readonly IViewUtils _viewUtils;
        private readonly ITodoManager _manager;
        private readonly IEnumerable<TodoItem> _todoItems;

        private int pendingColumnScroll;
        private int doneColumnScroll;
        private bool isPendingColumnSelected;

        public TodoView(ITodoManager manager, IViewUtils viewUtils, ICharacterDisplay characterDisplay)
        {
            pendingColumnScroll = 0;
            doneColumnScroll = 0;
            isPendingColumnSelected = true;

            _manager = manager;
            _characterDisplay = characterDisplay;
            _viewUtils = viewUtils;

            _todoItems = _manager.GetTodoItems().OrderByDescending(todo => todo.DueDate);

            _viewUtils.ClearAndWriteControls();
            _characterDisplay.CursorVisible = false;

            WriteHeader();
            Write();
        }

        private TodoItem[] TodosDone { get => [.. _todoItems.Where(todo => todo.IsDone)]; }
        private TodoItem[] TodosPending { get => [.. _todoItems.Where(todo => !todo.IsDone)]; }

        public void HandleKey(ConsoleKeyInfo keyInfo)
        {
            bool shouldRewrite = false;

            if (keyInfo.Key == ConsoleKey.RightArrow && isPendingColumnSelected)
            {
                isPendingColumnSelected = false;
                shouldRewrite = true;
            }
            else if (keyInfo.Key == ConsoleKey.LeftArrow && !isPendingColumnSelected)
            {
                isPendingColumnSelected = true;
                shouldRewrite = true;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow || keyInfo.Key == ConsoleKey.UpArrow)
            {
                shouldRewrite |= HandleScroll(keyInfo.Key == ConsoleKey.UpArrow);
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                var maybeTodo = FindSelectedTodo();

                if (maybeTodo is not null)
                {
                    TodoItem updatedTodoItem = new TodoItem() { Title = maybeTodo.Title, Description = maybeTodo.Description, DueDate = maybeTodo.DueDate, IsDone = !maybeTodo.IsDone };

                    _manager.Update(maybeTodo, updatedTodoItem);

                    ClampScroll();

                    shouldRewrite = true;
                }
            }


            if (shouldRewrite)
            {
                Write();
            }
        }

        private void ClampScroll()
        {
            if (isPendingColumnSelected && pendingColumnScroll > TodosPending.Length - 1)
            {
                pendingColumnScroll = Math.Max(0, TodosPending.Length - 1);
            }
            else if (!isPendingColumnSelected && doneColumnScroll > TodosDone.Length - 1)
            {
                doneColumnScroll = Math.Max(0, TodosDone.Length - 1);
            }
        }

        private bool HandleScroll(bool up)
        {
            if (isPendingColumnSelected)
            {
                return ScrollColumn(ref pendingColumnScroll, up, TodosPending.Length - 1);
            }

            return ScrollColumn(ref doneColumnScroll, up, TodosDone.Length - 1);
        }

        private void Write()
        {
            _viewUtils.ClearRegion(0, 1, _characterDisplay.Width, _characterDisplay.Height - 2);

            int columnWidth = _characterDisplay.Width / 2 - 1;
            int columnHeight = _characterDisplay.Height - 3;
            int lineNum = 0;

            var pendingTextColor = isPendingColumnSelected ? ConsoleColor.White : ConsoleColor.Black;

            string pendingText = "Pending";
            Console.SetCursorPosition(0, 1);

            _viewUtils.WrapWithColors(() =>
            {
                _viewUtils.WriteCenteredText(pendingText, columnWidth);
            }, foregroundColor: ConsoleColor.Red, backgroundColor: pendingTextColor);

            foreach (var item in TodosPending.Skip(pendingColumnScroll).Take(columnHeight))
            {
                bool isSelected = lineNum == 0 && isPendingColumnSelected;
                _viewUtils.WriteTodo(item, isSelected, 0, 2 + lineNum, columnWidth);

                lineNum++;
            }

            var doneTextColor = !isPendingColumnSelected ? ConsoleColor.White : ConsoleColor.Black;

            string doneText = "Done";
            _characterDisplay.SetCursorPosition(columnWidth + 1, 1);

            _viewUtils.WrapWithColors(() =>
            {
                _viewUtils.WriteCenteredText(doneText, columnWidth);
            }, foregroundColor: ConsoleColor.Green, backgroundColor: doneTextColor);

            lineNum = 0;
            foreach (var item in TodosDone.Skip(doneColumnScroll).Take(columnHeight))
            {
                bool isSelected = lineNum == 0 && !isPendingColumnSelected;
                _viewUtils.WriteTodo(item, isSelected, columnWidth + 1, 2 + lineNum, columnWidth);

                lineNum++;
            }

            WriteSeparator(columnWidth, 1, columnHeight, isPendingColumnSelected ? pendingColumnScroll : doneColumnScroll, isPendingColumnSelected ? TodosPending.Length : TodosDone.Length);
        }

        private void WriteHeader()
        {
            _viewUtils.WrapWithColors(() =>
            {
                var text = "Viewing todos";
                _characterDisplay.SetCursorPosition(_characterDisplay.Width / 2 - text.Length / 2, 0);
                _characterDisplay.WriteLine(text);
            }, foregroundColor: ConsoleColor.Magenta);
        }

        private bool ScrollColumn(ref int columnScroll, bool up, int maxScroll)
        {
            if (up && columnScroll > 0)
            {
                columnScroll--;
                return true;
            }
            else if (!up && columnScroll < maxScroll)
            {
                columnScroll++;
                return true;
            }

            return false;
        }

        private void WriteSeparator(int x, int y, int height, int scrollValue, int maxScrollValue)
        {
            float scrollPercentage = maxScrollValue > 0 ? scrollValue / (float)maxScrollValue : 0;

            bool scrollWritten = false;
            for (int i = 0; i <= height; i++)
            {
                _characterDisplay.SetCursorPosition(x, y + i);

                float columnPercent = i / (float)height;

                if (!scrollWritten && columnPercent >= scrollPercentage)
                {
                    scrollWritten = true;

                    _viewUtils.WrapWithColors(() =>
                    {
                        _characterDisplay.Write(SeparatorCharacter);
                    }, foregroundColor: ConsoleColor.Magenta);
                }
                else
                {
                    _characterDisplay.Write(SeparatorCharacter);
                }

            }
        }

        private TodoItem? FindSelectedTodo()
        {
            if (isPendingColumnSelected)
            {
                return TodosPending.Skip(pendingColumnScroll).Take(1).FirstOrDefault();
            }

            return TodosDone.Skip(doneColumnScroll).Take(1).FirstOrDefault();
        }
    }
}
