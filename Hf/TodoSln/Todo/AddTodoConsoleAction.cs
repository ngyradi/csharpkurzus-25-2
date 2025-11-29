using Todo.Core;

namespace Todo
{
    internal class AddTodoConsoleAction(ITodoManager manager) : IConsoleAction
    {
        public Result<string, string> Execute(string input)
        {
            var splits = input.Split(";");

            if (splits.Length != 3)
            {
                return new Result<string, string>(error: "Malformed input received");
            }

            if(!DateTime.TryParse(splits[2], out DateTime dueDate))
            {
                return new Result<string, string>(error: "Invalid date received");
            }

            TodoItem item = new() { Title = splits[0], Description = splits[1], DueDate = dueDate };
            manager.Add(item);

            return new Result<string, string>(success: "Todo successfully created");
        }
    }
}
