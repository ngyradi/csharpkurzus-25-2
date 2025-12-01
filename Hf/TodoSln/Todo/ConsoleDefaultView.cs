
using Todo.Core;

namespace Todo
{
    internal class ConsoleDefaultView : IConsoleView
    {
        private readonly ITodoManager _manager;

        public ConsoleDefaultView(ITodoManager manager)
        {
            _manager = manager;

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

            var currentFgColor = Console.ForegroundColor;
            var currentBgColor = Console.BackgroundColor;

            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;

            int columnWidth = ConsoleUI.Width / 2;
            Console.SetCursorPosition(0, 1);
            Console.WriteLine("Upcoming todos".PadRight(columnWidth));

            Console.ForegroundColor = currentFgColor;
            Console.BackgroundColor = currentBgColor;


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

            var currentFgColor = Console.ForegroundColor;
            var currentBgColor = Console.BackgroundColor;

            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;

            int columnWidth = ConsoleUI.Width / 2;

            int lineNum = 0;
            Console.SetCursorPosition(columnWidth, 1);
            Console.WriteLine("Last completed by month".PadLeft(columnWidth));
            foreach (var group in lastCompletedByMonth)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition(columnWidth, 2 + lineNum++);
                Console.Write(group.Month.PadRight(columnWidth));

                Console.BackgroundColor = currentBgColor;

                foreach (var item in group.Items)
                {
                    ConsoleUI.WriteTodo(item, false, columnWidth, 2 + lineNum++, columnWidth);
                }
            }

            Console.ForegroundColor = currentFgColor;
        }
    }
}
