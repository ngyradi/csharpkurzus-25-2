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

            WrapWithColors(() =>
            {
                Console.SetCursorPosition(0, Height - 1);

                string text = $"^C - Close{_whitespace}^A - Add todo{_whitespace}^W - View todos{_whitespace}^K - Save changes{_whitespace}^X - Clear";

                Console.Write(text.PadRight(Width));

                Console.SetCursorPosition(0, 0);
            }, ConsoleColor.Black, ConsoleColor.White);

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

        public static void WriteCenteredText(string text, int width)
        {
            Console.Write(text.PadRight(width / 2 + text.Length / 2).PadLeft(width));
        }

        public static void WrapWithColors(Action wrappedAction, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
        {
            var currentFgColor = Console.ForegroundColor;
            var currentBgColor = Console.BackgroundColor;

            if (foregroundColor is ConsoleColor fgColor)
            {
                Console.ForegroundColor = fgColor;
            }

            if (backgroundColor is ConsoleColor bgColor)
            {
                Console.BackgroundColor = bgColor;
            }

            wrappedAction();

            Console.ForegroundColor = currentFgColor;
            Console.BackgroundColor = currentBgColor;
        }

        public static void WriteTodo(TodoItem todo, bool isSelected, int x, int y, int maxWidth)
        {
            var backgroundColor = isSelected ? ConsoleColor.DarkGray : ConsoleColor.Black;

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

            WrapWithColors(() =>
            {
                Console.SetCursorPosition(x, y);
                Console.Write(new string(' ', maxWidth));
            }, backgroundColor: backgroundColor);

            WrapWithColors(() =>
            {
                Console.SetCursorPosition(x, y);
                Console.Write(title);
            }, ConsoleColor.White, backgroundColor);

            WrapWithColors(() =>
            {
                Console.Write(description);

                Console.SetCursorPosition(x + maxWidth - dueDate.Length, y);
                Console.Write(dueDate);
            }, ConsoleColor.Gray, backgroundColor);
        }
    }
}
