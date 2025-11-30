namespace Todo.Core
{
    public interface ITodoManager
    {
        Result<string, string> Save();
        void Load();
        void Add(TodoItem item);
        void Update(TodoItem original, TodoItem updated);
        IEnumerable<TodoItem> GetTodoItems();
    }
}