using Todo.Core;

namespace Todo.Views
{
    internal class ConsoleDefaultView : IConsoleView
    {
        private readonly ITodoManager _manager;

        public ConsoleDefaultView(ITodoManager manager)
        {
            _manager = manager;

            Console.CursorVisible = false;
            Write(_manager.GetTodoItems());
        }

        public void HandleKey(ConsoleKeyInfo keyInfo)
        {

        }

        private void Write(IEnumerable<TodoItem> todos)
        {
            WriteUpcomingTodos(todos);
            WriteLastCompletedByMonthTodos(todos);
        }

        private void WriteUpcomingTodos(IEnumerable<TodoItem> todos)
        {
            var upcomingTodos = todos
                .Where(t => !t.IsDone && t.DueDate >= DateTime.Now)
                .OrderByDescending(t => t.DueDate)
                .Take(3);

            int columnWidth = ConsoleUI.Width / 2;

            ConsoleUI.WrapWithColors(() =>
            {
                Console.SetCursorPosition(0, 1);

                ConsoleUI.WriteCenteredText("Upcoming todos", columnWidth);
            }, ConsoleColor.Black, ConsoleColor.White);

            int lineNum = 0;

            if (upcomingTodos.Count() == 0)
            {
                Console.WriteLine("No upcoming todos");
            }
            else
            {
                foreach (var item in upcomingTodos)
                {
                    ConsoleUI.WriteTodo(item, false, 0, 2 + lineNum++,columnWidth);
                }
            }
        }

        private void WriteLastCompletedByMonthTodos(IEnumerable<TodoItem> todos)
        {
            var lastCompletedByMonth = todos
             .Where(t => t.IsDone)
             .GroupBy(t => new { t.DueDate.Year, t.DueDate.Month })
             .OrderByDescending(g => g.Key.Year)
             .ThenByDescending(g => g.Key.Month)
             .Select(g => new
             {
                 Month = $"{new DateTime(g.Key.Year, g.Key.Month, 1):yyyy MMMM}",
                 LastCompleted = g.Max(t => t.DueDate),
                 Items = g.OrderByDescending(t => t.DueDate).Take(3)
             });

            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;

            int columnWidth = ConsoleUI.Width / 2;

            int lineNum = 0;

            ConsoleUI.WrapWithColors(() =>
            {
                Console.SetCursorPosition(columnWidth, 1);

                ConsoleUI.WriteCenteredText("Last completed by month", columnWidth);
            }, ConsoleColor.Black, ConsoleColor.White);

            foreach (var group in lastCompletedByMonth)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Gray;

                ConsoleUI.WrapWithColors(() =>
                {
                    Console.SetCursorPosition(columnWidth, 2 + lineNum++);
                    Console.Write(group.Month.PadRight(columnWidth));
                }, ConsoleColor.Black, ConsoleColor.White);

                foreach (var item in group.Items)
                {
                    ConsoleUI.WriteTodo(item, false, columnWidth, 2 + lineNum++, columnWidth);
                }
            }
        }
    }
}
