namespace Todo
{
    static internal class ConsoleUI
    {
        private const string _whitespace = "    ";
        public static int Height => Console.BufferHeight;

        public static int Width => Console.BufferWidth;

        public static InputMode InputMode { get; set; } = InputMode.List;

        public static void Clear()
        {
            Console.Clear();
            Console.SetCursorPosition(0, Height - 1);

            var currentBackground = Console.BackgroundColor;
            var currentForeground = Console.ForegroundColor;

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            string text = $"^C - Close{_whitespace}^A - Add todo{_whitespace}^X - View todos{_whitespace}^K - Save changes";

            Console.Write(text);
            for (int i = 0; i < Width - text.Length; i++)
            {
                Console.Write(' ');
            }

            Console.SetCursorPosition(0, 0);

            Console.BackgroundColor = currentBackground;
            Console.ForegroundColor = currentForeground;

            WriteInfoText();
        }

        private static void WriteInfoText()
        {
            var currentForeground = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Magenta;

            if (InputMode == InputMode.List)
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
