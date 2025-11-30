using Todo.Core;

namespace Todo
{
    internal interface IConsoleTodoView
    {
        void WriteTodos(IEnumerable<TodoItem> todos);
    }

    internal class ConsoleTodoView : IConsoleTodoView
    {
        int pendingColumnScroll = 0;
        int doneColumnScroll = 0;

        public void WriteTodos(IEnumerable<TodoItem> todos)
        {
            var todoGroupsByIsDone = todos.OrderByDescending(todo => todo.DueDate)
                .GroupBy(todo => todo.IsDone);

            var todosDone = todoGroupsByIsDone
                .Where(g => g.Key == true).Select(g => g.ToArray()).FirstOrDefault() ?? [];
            var todosPending = todoGroupsByIsDone
                .Where(g => g.Key == false).Select(g => g.ToArray()).FirstOrDefault() ?? [];

            int columnWidth = ConsoleUI.Width / 2 - 1;
            int columnHeight = ConsoleUI.Height - 3;
            int lineNum = 0;

            string pendingText = "Pending";
            Console.SetCursorPosition(0, 1);
            Console.WriteLine(pendingText.PadLeft(columnWidth));
            foreach ( var item in todosPending.Skip(pendingColumnScroll).Take(columnHeight))
            {
                Console.SetCursorPosition(0, 2 + lineNum++);

                var text = item.Title;

                Console.WriteLine(text);
            }

            string doneText = "Done";
            Console.SetCursorPosition(columnWidth + 1, 1);
            Console.WriteLine(doneText.PadRight(columnWidth));

            lineNum = 0;
            foreach (var item in todosDone.Skip(doneColumnScroll).Take(columnHeight)){
                Console.SetCursorPosition(columnWidth + 1,2+lineNum++);

                var text = item.Title;

                Console.Write(text);
            }

            for (int i =0; i <= columnHeight; i++)
            {
                Console.SetCursorPosition(columnWidth, 1+i);
                Console.Write('┃');
            }
        }
    }
}
