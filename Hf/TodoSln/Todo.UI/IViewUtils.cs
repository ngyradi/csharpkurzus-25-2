using Todo.Core;

namespace Todo.UI
{
    public interface IViewUtils
    {
        void ClearAndWriteControls();
        void ClearRegion(int startX, int startY, int width, int height);
        void WrapWithColors(Action wrappedAction, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null);
        void WriteCenteredText(string text, int width);
        void WriteTodo(TodoItem todo, bool isSelected, int x, int y, int maxWidth);
    }
}