using System.Diagnostics;
using Todo.Core;

namespace Todo
{
    internal class ConsoleTodoView : IConsoleView
    {
        private readonly ITodoManager _manager;

        int pendingColumnScroll;
        int doneColumnScroll;
        bool isPendingColumnSelected;

        IEnumerable<TodoItem> _todoItems;

        public ConsoleTodoView(ITodoManager manager)
        {
            pendingColumnScroll = 0;
            doneColumnScroll = 0;
            isPendingColumnSelected = true;

            _manager = manager;

            _todoItems = _manager.GetTodoItems().OrderByDescending(todo => todo.DueDate);

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

            if (keyInfo.Key == ConsoleKey.LeftArrow && !isPendingColumnSelected)
            {
                isPendingColumnSelected = true;
                shouldRewrite = true;
            }

            if (keyInfo.Key == ConsoleKey.DownArrow || keyInfo.Key == ConsoleKey.UpArrow)
            {
                shouldRewrite |= HandleScroll(keyInfo.Key == ConsoleKey.UpArrow);
            }

            if (keyInfo.Key == ConsoleKey.Enter)
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
            if (isPendingColumnSelected && pendingColumnScroll > TodosPending.Length-1)
            {
                pendingColumnScroll = Math.Max(0, TodosPending.Length - 1);
            }
            else if (!isPendingColumnSelected && doneColumnScroll > TodosDone.Length-1)
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
            ConsoleUI.ClearRegion(0, 1, ConsoleUI.Width, ConsoleUI.Height - 2);


            int columnWidth = ConsoleUI.Width / 2 - 1;
            int columnHeight = ConsoleUI.Height - 3;
            int lineNum = 0;

            var currentBgColor = Console.BackgroundColor;
            var currentFgColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
            if (isPendingColumnSelected)
            {
                Console.BackgroundColor = ConsoleColor.White;
            }

            string pendingText = "Pending";
            Console.SetCursorPosition(0, 1);

            Console.WriteLine(pendingText.PadLeft(columnWidth));

            Console.BackgroundColor = currentBgColor;
            Console.ForegroundColor = currentFgColor;

            foreach (var item in TodosPending.Skip(pendingColumnScroll).Take(columnHeight))
            {
                bool isSelected = lineNum == 0 && isPendingColumnSelected;
                WriteTodo(item, isSelected, 0, 2 + lineNum, columnWidth);

                lineNum++;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            if (!isPendingColumnSelected)
            {
                Console.BackgroundColor = ConsoleColor.White;
            }

            string doneText = "Done";
            Console.SetCursorPosition(columnWidth + 1, 1);
            Console.WriteLine(doneText.PadRight(columnWidth));

            Console.BackgroundColor = currentBgColor;
            Console.ForegroundColor = currentFgColor;

            lineNum = 0;
            foreach (var item in TodosDone.Skip(doneColumnScroll).Take(columnHeight))
            {
                bool isSelected = lineNum == 0 && !isPendingColumnSelected;
                WriteTodo(item, isSelected, columnWidth + 1, 2 + lineNum, columnWidth);

                lineNum++;
            }

            WriteSeparator(columnWidth, 1, columnHeight, isPendingColumnSelected ? pendingColumnScroll : doneColumnScroll, isPendingColumnSelected ? TodosPending.Length : TodosDone.Length);
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

        private void WriteTodo(TodoItem todo, bool isSelected, int x, int y, int maxWidth)
        {
            var currentBgColor = Console.BackgroundColor;

            if (isSelected)
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
            }

            string title = isSelected ? $">{todo.Title}" : todo.Title;
            string description = todo.Description;
            string dueDate = todo.DueDate.ToShortDateString();

            title += " ";
            description += " ";

            int remainingWidth = maxWidth - dueDate.Length;

            if (title.Length > remainingWidth / 2)
            {
                title = string.Concat(title.AsSpan(0, remainingWidth / 2 - 4), "... ");
            }

            if (description.Length > remainingWidth / 2)
            {
                description = string.Concat(description.AsSpan(0, remainingWidth / 2 - 4), "... ");
            }

            dueDate = dueDate.PadLeft(maxWidth - (title.Length + description.Length), ' ');

            Console.SetCursorPosition(x, y);
            Console.Write(new string(' ', maxWidth));

            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(title);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(description);

            Console.SetCursorPosition(x + maxWidth - dueDate.Length, y);
            Console.Write(dueDate);

            Console.BackgroundColor = currentBgColor;
        }

        private void WriteSeparator(int x, int y, int height, int scrollValue, int maxScrollValue)
        {
            float scrollPercentage = maxScrollValue > 0 ? (scrollValue) / (float)(maxScrollValue) : 0;

            var currentFgColor = Console.ForegroundColor;

            bool scrollWritten = false;
            for (int i = 0; i <= height; i++)
            {
                float columnPercent = i / (float)height;

                if (!scrollWritten && columnPercent >= scrollPercentage)
                {
                    scrollWritten = true;
                    Console.ForegroundColor = ConsoleColor.Magenta;
                }
                else
                {
                    Console.ForegroundColor = currentFgColor;
                }

                Console.SetCursorPosition(x, y + i);
                Console.Write('┃');
            }

            Console.ForegroundColor = currentFgColor;
        }

        private TodoItem? FindSelectedTodo()
        {
            if (isPendingColumnSelected)
            {
                return TodosPending.Skip(pendingColumnScroll).Take(1).FirstOrDefault();
            }
            else
            {
                return TodosDone.Skip(doneColumnScroll).Take(1).FirstOrDefault();
            }
        }
    }
}
