
namespace Todo.UI
{
    public interface ICharacterDisplay
    {
        ConsoleColor BackgroundColor { get; set; }
        ConsoleColor ForegroundColor { get; set; }
        bool CursorVisible { get; set; }
        int CursorLeft { get; set; }
        int CursorTop { get; set; }
        int Height { get; }
        int Width { get; }

        void Clear();
        void ResetColor();
        void SetCursorPosition(int x, int y);
        void Write(char character);
        void Write(string text);
        void WriteLine(char character);
        void WriteLine(string text);
    }
}