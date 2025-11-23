using Todo.Core;

namespace Todo
{
    internal class AddTodoConsoleAction(ITodoManager manager) : IConsoleAction
    {
        public void Execute(string input)
        {
            var splits = input.Split(";");

            if (splits.Length != 3)
            {
                throw new ArgumentException("Invalid input received");
            }

            if(!DateTime.TryParse(splits[2], out DateTime dueDate))
            {
                throw new ArgumentException("Invalid date received");
            }

            TodoItem item = new() { Title = splits[0], Description = splits[1], DueDate = dueDate };
            manager.Add(item);
        }
    }

    internal class SaveChangesConsoleAction(ITodoManager manager) : IConsoleAction
    {
        public void Execute(string input) {
            if (input.Equals("y"))
            {
                manager.Save();
            }
        }
    }
}
