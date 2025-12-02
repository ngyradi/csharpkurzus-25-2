using Todo.Core;

namespace Todo.UI
{
    public static class ViewFactory
    {
        public static ICharacterView CreateView(InputMode inputMode, ITodoManager manager, IViewUtils viewUtils, ICharacterDisplay characterDisplay, TimeProvider timeProvider)
        {
            return inputMode switch
            {
                InputMode.None => new DefaultView(manager, viewUtils, characterDisplay, timeProvider),
                InputMode.Adding => new AddTodoView(manager, viewUtils, characterDisplay),
                InputMode.Saving => new SaveView(manager, viewUtils, characterDisplay),
                InputMode.Listing => new TodoView(manager, viewUtils, characterDisplay),
                _ => throw new InvalidOperationException("Branch for set mode does not exist"),
            };
        }
    }
}
