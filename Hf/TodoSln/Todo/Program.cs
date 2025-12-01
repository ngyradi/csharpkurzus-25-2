using System.Text;
using Todo.Core;
using Todo.UI;

namespace Todo
{

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.TreatControlCAsInput = true;
            Console.OutputEncoding = Encoding.UTF8;

            var savePath = Path.Combine(AppContext.BaseDirectory, "save.json");
            
            ITodoManager manager = new JsonTodoManager(savePath);
            manager.Load();

            var consoleDisplay = new ConsoleDisplay();
            var consoleViewUtils = new ConsoleViewUtils();
            var consoleKeyHandler = new ConsoleModeHandler(manager, consoleViewUtils, consoleDisplay);

            ConsoleKeyInfo keyInfo;
            do
            {
               keyInfo = Console.ReadKey();
            } while (consoleKeyHandler.Handle(keyInfo));
        }
    }
}