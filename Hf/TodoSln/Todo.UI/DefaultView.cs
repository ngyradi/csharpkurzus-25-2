using Todo.Core;

namespace Todo.UI
{
    internal class DefaultView : ICharacterView
    {
        private readonly IViewUtils _viewUtils;
        private readonly ICharacterDisplay _characterDisplay;
        private readonly ITodoManager _manager;

        public DefaultView(ITodoManager manager, IViewUtils viewUtils, ICharacterDisplay characterDisplay)
        {
            _manager = manager;
            _viewUtils = viewUtils;
            _characterDisplay = characterDisplay;

            _viewUtils.ClearAndWriteControls();
            _characterDisplay.CursorVisible = false;

            WriteHeader();
            Write(_manager.GetTodoItems());
        }

        public void HandleKey(ConsoleKeyInfo keyInfo)
        {

        }

        private void WriteHeader()
        {
            _viewUtils.WrapWithColors(() =>
            {

                var text = "Overview";
                _characterDisplay.SetCursorPosition(_characterDisplay.Width / 2 - text.Length / 2, 0);
                _characterDisplay.WriteLine(text);
            }, foregroundColor: ConsoleColor.Magenta);
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

            int columnWidth = _characterDisplay.Width / 2;

            _viewUtils.WrapWithColors(() =>
            {
                _characterDisplay.SetCursorPosition(0, 1);

                _viewUtils.WriteCenteredText("Upcoming todos", columnWidth);
            }, ConsoleColor.Black, ConsoleColor.White);

            int lineNum = 0;

            if (upcomingTodos.Count() == 0)
            {
                _characterDisplay.WriteLine("No upcoming todos");
            }
            else
            {
                foreach (var item in upcomingTodos)
                {
                    _viewUtils.WriteTodo(item, false, 0, 2 + lineNum++,columnWidth);
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

            _characterDisplay.ForegroundColor = ConsoleColor.Black;
            _characterDisplay.BackgroundColor = ConsoleColor.White;

            int columnWidth = _characterDisplay.Width / 2;

            int lineNum = 0;

            _viewUtils.WrapWithColors(() =>
            {
                _characterDisplay.SetCursorPosition(columnWidth, 1);

                _viewUtils.WriteCenteredText("Last completed by month", columnWidth);
            }, ConsoleColor.Black, ConsoleColor.White);

            foreach (var group in lastCompletedByMonth)
            {
                _characterDisplay.ForegroundColor = ConsoleColor.Black;
                _characterDisplay.BackgroundColor = ConsoleColor.Gray;

                _viewUtils.WrapWithColors(() =>
                {
                    _characterDisplay.SetCursorPosition(columnWidth, 2 + lineNum++);
                    _characterDisplay.Write(group.Month.PadRight(columnWidth));
                }, ConsoleColor.Black, ConsoleColor.White);

                foreach (var item in group.Items)
                {
                    _viewUtils.WriteTodo(item, false, columnWidth, 2 + lineNum++, columnWidth);
                }
            }
        }
    }
}
