using Todo.Core;

namespace Todo
{
    internal interface IConsoleAction
    {
        Result<string, string> Execute(string input);
    }
}
