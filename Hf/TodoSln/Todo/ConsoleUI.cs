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
            Console.Clear();
            Console.SetCursorPosition(0, Console.BufferHeight - 1);
            Console.Write($"^Q - Quit{_whitespace}^A - Add Todo{_whitespace}^X - Clear mode{_whitespace}^K - Save changes");
            Console.SetCursorPosition(0, 0);

            WriteInfoText();
        }

        private static void WriteInfoText()
        {
            if (InputMode == InputMode.Adding)
            {
                var text = "Adding new Todo";
                Console.SetCursorPosition((Console.BufferWidth / 2) - (text.Length / 2), 0);
                Console.WriteLine(text);
                Console.WriteLine("Title;Description;DueDate");
                Console.CursorLeft = "Title;".Length;
                Console.Write("[Optional]\n");
            } else if (InputMode == InputMode.Saving)
            {
                var text = "Are you sure you want to save the changes?";
                Console.SetCursorPosition((Console.BufferWidth / 2) - (text.Length / 2), 0);
                Console.WriteLine(text);
            }
        }
    }
}
