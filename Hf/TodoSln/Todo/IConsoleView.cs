using Todo.Core;

namespace Todo
{
    internal interface IConsoleView
    {
        void HandleKey(ConsoleKeyInfo keyInfo);
    }
}
