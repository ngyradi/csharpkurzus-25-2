namespace Todo.Core
{
    public interface ITodoManager
    {
        void Save();
        void Load();
        void Add(TodoItem item);
        IEnumerable<TodoItem> GetTodoItems();
    }
}