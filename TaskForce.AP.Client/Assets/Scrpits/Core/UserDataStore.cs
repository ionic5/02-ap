namespace TaskForce.AP.Client.Core
{
    public class UserDataStore
    {
        private int _gold;

        public UserDataStore()
        {
            _gold = 999;
        }

        public int GetGold()
        {
            return _gold;
        }
    }
}
