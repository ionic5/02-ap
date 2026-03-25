namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class BattleLog
    {
        private float _battleTime;
        private int _killCount;

        public float BattleTime => _battleTime;
        public int KillCount => _killCount;

        public void AddKillCount(int value = 1)
        {
            _killCount += value;
        }

        public void AddBattleTime(float deltaTime)
        {
            _battleTime += deltaTime;
        }
    }
}
