using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Todo.Core
{
    public class JsonTodoManager(string savePath) : ITodoManager
    {
        private List<TodoItem> _items = [];

        public void Add(TodoItem item)
        {
            _items.Add(item);
        }

        public IEnumerable<TodoItem> GetTodoItems()
        {
            return _items;
        }

        public void Load()
        {
            if (!File.Exists(savePath))
            {
                return;
            }

            try
            {

                var file = File.ReadAllBytes(savePath);

                var items = JsonSerializer.Deserialize<TodoItem[]>(file) ?? [];

                _items = items.ToList();
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"Failed to load save file: {ex.Message}");
            }
        }

        public Result<string, string> Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(_items);

                try
                {

                    File.WriteAllText(savePath, json);
                    return new Result<string, string>(success: "Save successful");
                }
                catch (IOException ex)
                {
                    return new Result<string, string>(error: $"Failed to save file: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return new Result<string, string>(error: $"Unexpected error ocurred while saving: {ex.Message}");
            }
        }
    }
}