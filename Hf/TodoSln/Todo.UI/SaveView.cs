using Todo.Core;

namespace Todo.UI
{
    internal class SaveView: ICharacterView
    {
        private readonly ICharacterDisplay _characterDisplay;
        private readonly IViewUtils _viewUtils;
        private readonly ITodoManager _manager;

        public SaveView(ITodoManager manager,IViewUtils viewUtils, ICharacterDisplay characterDisplay)
        {
            _manager = manager;
            _characterDisplay = characterDisplay;
            _viewUtils = viewUtils;

            _viewUtils.ClearAndWriteControls();
            _characterDisplay.CursorVisible = true;

            WriteHeader();
            Write();
        }

        public void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key != ConsoleKey.Y)
            {
                Write();
                return;
            }

            _characterDisplay.CursorLeft--;
            _characterDisplay.Write(' ');

            Write();

            var result = _manager.Save();

            if (result.Success is not null)
            {
                _viewUtils.WrapWithColors(() =>
                {
                    _characterDisplay.WriteLine(result.Success);

                }, foregroundColor: ConsoleColor.DarkGreen);
            }else if (result.Error is not null)
            {
                _viewUtils.WrapWithColors(() =>
                {
                    _characterDisplay.WriteLine(result.Error);
                }, foregroundColor: ConsoleColor.Red);
            }
        }

        private void Write()
        {
            _viewUtils.ClearRegion(0, 1, _characterDisplay.Width, 3);
            _characterDisplay.SetCursorPosition(0, 1);
            _characterDisplay.WriteLine("Type 'y' to save");
        }

        private void WriteHeader()
        {
            _viewUtils.WrapWithColors(() =>
            {
                var text = "Are you sure you want to save the changes?";
                _characterDisplay.SetCursorPosition(_characterDisplay.Width / 2 - text.Length / 2, 0);
                _characterDisplay.WriteLine(text);
            }, foregroundColor: ConsoleColor.Magenta);
        }
    }
}
