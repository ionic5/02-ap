using System;
using System.Collections.Generic;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class WindowOpener
    {
        private readonly IWindowStack _windowStack;
        private readonly IWorld _world;
        private readonly TextStore _textStore;
        private readonly ISoundPlayer _soundPlayer;
        private readonly ILogger _logger;

        public WindowOpener(IWindowStack windowStack, IWorld world, TextStore textStore, ISoundPlayer soundPlayer, ILogger logger)
        {
            _windowStack = windowStack;
            _world = world;
            _textStore = textStore;
            _soundPlayer = soundPlayer;
            _logger = logger;
        }

        public void OpenPerkSelectionWindow(Entity.Unit unit, IEnumerable<Entity.ISkill> skills)
        {
            var window = _windowStack.OpenSkillSelectionWindow();
            OnWindowOpened(window);

            var ctrl = new SkillSelectionWindowController(window, skills, unit, _textStore);
            ctrl.Start();
        }

        public void OpenSettingWindow()
        {
            var window = _windowStack.OpenSettingWindow();
            OnWindowOpened(window);

            var ctrl = new SettingWindowController(window, _soundPlayer);
            ctrl.Start();
        }

        private void OnWindowOpened(IWindow window)
        {
            if (_windowStack.GetOpenedWindowCount() == 1)
                _world.Pause();

            EventHandler handler = null;
            handler = (sender, e) =>
            {
                window.ClosedEvent -= handler;

                if (_windowStack.GetOpenedWindowCount() == 0)
                    _world.Resume();
            };

            window.ClosedEvent += handler;
        }
    }
}
