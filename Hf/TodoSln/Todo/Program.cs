using System.Text;
using Todo.Core;

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

            IConsoleKeyHandler keyHandler = new ConsoleKeyHandler(manager);

            ConsoleUI.Clear();


            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey();
            } while (keyHandler.Handle(keyInfo));
        }
    }
}