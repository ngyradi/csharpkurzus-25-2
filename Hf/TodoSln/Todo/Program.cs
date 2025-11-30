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

            IConsoleTodoView todoView = new ConsoleTodoView();

            IConsoleKeyHandler keyHandler = new ConsoleKeyHandler(manager, todoView);

            ConsoleUI.Clear();


            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey();
            } while (keyHandler.Handle(keyInfo));
        }
    }
}