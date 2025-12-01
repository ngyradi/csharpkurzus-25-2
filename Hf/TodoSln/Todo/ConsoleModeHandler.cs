using System.Diagnostics;
using Todo.Core;
using Todo.UI;

namespace Todo
{
    internal class ConsoleModeHandler
    {
        private readonly ICharacterDisplay _characterDisplay;
        private readonly IViewUtils _viewUtils;
        private readonly ITodoManager _manager;
        private ICharacterView? _view = null;

        public ConsoleModeHandler(ITodoManager manager, IViewUtils viewUtils, ICharacterDisplay characterDisplay)
        {
            _manager = manager;
            _characterDisplay = characterDisplay;
            _viewUtils = viewUtils;

            _view = SwitchInputMode(InputMode.None);
        }

        public bool Handle(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.C && keyInfo.Modifiers == ConsoleModifiers.Control)
            {
                return false;
            }

            if (keyInfo.Modifiers == ConsoleModifiers.Control)
            {
                HandleControlKey(keyInfo);
            }

            _view?.HandleKey(keyInfo);

            return true;
        }

        private void HandleControlKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.A)
            {
                _view = SwitchInputMode(InputMode.Adding);
            }

            if (keyInfo.Key == ConsoleKey.X)
            {
                _view = SwitchInputMode(InputMode.None);
            }

            if (keyInfo.Key == ConsoleKey.W)
            {
                _view = SwitchInputMode(InputMode.Listing);
            }

            if (keyInfo.Key == ConsoleKey.K)
            {
                _view = SwitchInputMode(InputMode.Saving);
            }
        }

        private ICharacterView SwitchInputMode(InputMode inputMode)
        {
            return ViewFactory.CreateView(inputMode, _manager, _viewUtils, _characterDisplay);
        }
    }
}