using System;
using TaskForce.AP.Client.Core.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using IWindowStack = TaskForce.AP.Client.Core.View.LobbyScene.IWindowStack;

namespace TaskForce.AP.Client.Core.LobbyScene
{
    public class WindowOpener
    {
        private readonly IWindowStack _windowStack;
        private readonly TextStore _textStore;
        private readonly ISoundPlayer _soundPlayer;
        private readonly ILogger _logger;

        public WindowOpener(IWindowStack windowStack, TextStore textStore, ISoundPlayer soundPlayer, ILogger logger)
        {
            _windowStack = windowStack;
            _textStore = textStore;
            _soundPlayer = soundPlayer;
            _logger = logger;
        }

        public void OpenEnergyGetWindow(Action onEnergyGetConfirmed, string descriptionText)
        {
            var window = _windowStack.OpenEnergyGetWindow();
            window.SetDescription(descriptionText);
            var ctrl = new EnergyGetWindowController(window, onEnergyGetConfirmed);
            ctrl.Start();
        }

        public void OpenCommonWindow(string text)
        {
            var window = _windowStack.OpenCommonWindow();
            window.SetContentsText(text);

            var ctrl = new CommonWindowController(window);
            ctrl.Start();
        }

        public void OpenRankUpWindow(Action onRankUpConfirmed, string descriptionText)
        {
            var window = _windowStack.OpenRankUpWindow();
            window.SetDescription(descriptionText);

            var ctrl = new RankUpWindowController(window, onRankUpConfirmed);
            ctrl.Start();
        }

        public void OpenSettingWindow()
        {
            var window = _windowStack.OpenSettingWindow();

            var commonCtrl = new Core.SettingWindowController(window, _soundPlayer);
            var ctrl = new SettingWindowController(window, commonCtrl);
            ctrl.Start();
        }
    }
}
