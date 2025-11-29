using Todo.Core;

namespace Todo
{
    internal class SaveChangesConsoleAction(ITodoManager manager) : IConsoleAction
    {
        public Result<string, string> Execute(string input) {
            if (input.Equals("y"))
            {
                return manager.Save();
            }

            return new Result<string, string>(error: "Type 'y' to save");
        }
    }
}
