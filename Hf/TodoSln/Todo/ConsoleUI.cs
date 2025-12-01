using Todo.Core;

namespace Todo
{
    static internal class ConsoleUI
    {
        private const string _whitespace = "    ";
        public static int Height => Console.BufferHeight;

        public static int Width => Console.BufferWidth;

        public static void Clear()
        {
            Console.ResetColor();
            Console.CursorVisible = false;
            Console.Clear();
            Console.SetCursorPosition(0, Height - 1);

            var currentBackground = Console.BackgroundColor;
            var currentForeground = Console.ForegroundColor;

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            string text = $"^C - Close{_whitespace}^A - Add todo{_whitespace}^W - View todos{_whitespace}^K - Save changes{_whitespace}^X - Clear";

            Console.Write(text.PadRight(Width));

            Console.SetCursorPosition(0, 0);

            Console.BackgroundColor = currentBackground;
            Console.ForegroundColor = currentForeground;

            Console.CursorVisible = true;
        }

        public static void ClearRegion(int startX, int startY, int width, int height)
        {
            for (int y = startY; y < startY + height; y++)
            {
                Console.SetCursorPosition(startX, y);
                Console.Write(" ".PadRight(width));
            }
        }

        public static void WriteTodo(TodoItem todo, bool isSelected, int x, int y, int maxWidth)
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
    }
}
