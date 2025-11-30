
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
            var upcomingTodos = todos
                .Where(t => !t.IsDone && t.DueDate >= DateTime.Now)
                .OrderByDescending(t => t.DueDate)
                .Take(3);

            var lastCompletedByMonth = todos
                .Where(t => t.IsDone)
                .GroupBy(t => new { t.DueDate.Year, t.DueDate.Month })
                .OrderByDescending(g => g.Key.Year)
                .ThenByDescending(g => g.Key.Month)
                .Select(g => new
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    LastCompleted = g.Max(t => t.DueDate),
                    Items = g.OrderByDescending(t => t.DueDate).Take(3)
                });

            Console.SetCursorPosition(0, 1);
            Console.WriteLine("Upcoming todos");

            int lineNum = 0;
            foreach (var item in upcomingTodos)
            {
                ConsoleUI.WriteTodo(item, false, 0, 2 + lineNum++, ConsoleUI.Width/2);
            }

            lineNum = 0;
            Console.SetCursorPosition(ConsoleUI.Width / 2, 1);
            Console.WriteLine("Last completed by month");
            foreach (var group in lastCompletedByMonth)
            {
                Console.SetCursorPosition(ConsoleUI.Width / 2, 2 + lineNum++);
                Console.Write(group.Month);

                foreach(var item in group.Items)
                {
                    ConsoleUI.WriteTodo(item, false, ConsoleUI.Width / 2, 2 + lineNum++,ConsoleUI.Width/2);
                }
            }
        }
    }
}
