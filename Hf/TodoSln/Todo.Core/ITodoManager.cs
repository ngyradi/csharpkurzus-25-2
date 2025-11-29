namespace Todo.Core
{
    public interface ITodoManager
    {
        Result<string, string> Save();
        void Load();
        void Add(TodoItem item);
        IEnumerable<TodoItem> GetTodoItems();
    }
}