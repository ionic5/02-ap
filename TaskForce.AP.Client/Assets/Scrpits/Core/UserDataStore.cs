namespace TaskForce.AP.Client.Core
{
    public class UserDataStore
    {
        private int _gold;
        private int _energy;

        public UserDataStore()
        {
            _gold = 0;      // TODO: JW: 초기 지급 골드 적용
            _energy = 0;    // TODO: JW: 초기 지급 에너지 적용
        }

        public void SetGold(int gold)
        {
            _gold = gold;
        }
        
        public int GetGold()
        {
            return _gold;
        }

        public void SetEnergy(int energy)
        {
            _energy = energy;
        }

        public int GetEnergy()
        {
            return _energy;
        }
    }
}
