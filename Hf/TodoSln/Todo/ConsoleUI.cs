namespace Todo
{
    static internal class ConsoleUI
    {
        private const string _whitespace = "    ";
        public static int Height => Console.BufferHeight;

        public static int Width => Console.BufferWidth;

        public static InputMode InputMode { get; set; } = InputMode.None;

        public static void Clear()
        {
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

            WriteInfoText();
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

        private static void WriteInfoText()
        {
            var currentForeground = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Magenta;

            if (InputMode == InputMode.Listing)
            {
                var text = "Viewing todos";
                Console.SetCursorPosition((Width / 2) - (text.Length / 2), 0);
                Console.WriteLine(text);
            }
            else if (InputMode == InputMode.Adding)
            {
                var text = "Adding new Todo";
                Console.SetCursorPosition((Width / 2) - (text.Length / 2), 0);
                Console.WriteLine(text);

                Console.ForegroundColor = ConsoleColor.DarkBlue;

                Console.WriteLine("Title;Description;DueDate");
            }
            else if (InputMode == InputMode.Saving)
            {
                var text = "Are you sure you want to save the changes?";
                Console.SetCursorPosition((Width / 2) - (text.Length / 2), 0);
                Console.WriteLine(text);
            }

            Console.ForegroundColor = currentForeground;
        }
    }
}
