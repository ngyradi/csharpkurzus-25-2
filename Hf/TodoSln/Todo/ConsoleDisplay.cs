using Todo.UI;

namespace Todo
{
    internal class ConsoleDisplay : ICharacterDisplay
    {
        public int Width { get => Console.BufferWidth; }
        public int Height { get => Console.BufferHeight; }

        public ConsoleColor ForegroundColor
        {
            get => Console.ForegroundColor;
            set
            {
                Console.ForegroundColor = value;
            }
        }

        public ConsoleColor BackgroundColor
        {
            get => Console.BackgroundColor;
            set
            {
                Console.BackgroundColor = value;
            }
        }

        public bool CursorVisible
        {
            get => Console.CursorVisible;
            set
            {
                Console.CursorVisible = value;
            }
        }

        public int CursorLeft
        {
            get => Console.CursorLeft; set
            {
                Console.CursorLeft = value;
            }
        }
        public int CursorTop
        {
            get => Console.CursorTop; set
            {
                Console.CursorTop = value;
            }
        }

        public void Write(string text)
        {
            Console.Write(text);
        }

        public void Write(char character)
        {
            Console.Write(character);
        }

        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        public void WriteLine(char character)
        {
            Console.WriteLine(character);
        }

        public void ResetColor()
        {
            Console.ResetColor();
        }

        public void Clear()
        {
            Console.Clear();
        }

        public void SetCursorPosition(int x, int y)
        {
            Console.SetCursorPosition(x, y);
        }
    }
}
