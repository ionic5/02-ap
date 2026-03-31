namespace TaskForce.AP.Client.Core
{
    public class UserDataStore
    {
        private int _gold;
        private int _energy;
        private int _rank;

        public UserDataStore()
        {
            // TODO: JW: 초기 지급 값 적용
            _gold = 0;      
            _energy = 0;   
            _rank = 0;
            //
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

        public void SetRank(int rank)
        {
            _rank = rank;
        }

        public int GetRank()
        {
            return _rank;
        }
    }
}
