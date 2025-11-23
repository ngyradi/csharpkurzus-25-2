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
                return;
            }
        }

        public void Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(_items);

                try
                {
                    File.WriteAllText(savePath, json);
                }
                catch (IOException ex)
                {
                    // Not able to write file
                }
            }
            catch (Exception ex)
            {
                // Data is not able to be serialized
            }
        }
    }
}