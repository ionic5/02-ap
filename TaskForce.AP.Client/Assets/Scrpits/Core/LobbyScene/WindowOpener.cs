using TaskForce.AP.Client.Core.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using IWindowStack = TaskForce.AP.Client.Core.View.LobbyScene.IWindowStack;

namespace TaskForce.AP.Client.Core.LobbyScene
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

        public void OpenEnergyGetWindow()
        {
            var window = _windowStack.OpenEnergyGetWindow();
            
            var ctrl = new EnergyGetWindowController(window);
            ctrl.Start();
        }
        
        public void OpenCommonWindow()
        {
            var window = _windowStack.OpenCommonWindow();
            
            var ctrl = new CommonWindowController(window);
            ctrl.Start();
        }

        public void OpenRankUpWindow()
        {
            var window = _windowStack.OpenRankUpWindow();

            var ctlr = new RankUpWindowController(window);
            ctlr.Start();
        }
    }
}