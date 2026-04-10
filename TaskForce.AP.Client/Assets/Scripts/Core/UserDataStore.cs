namespace TaskForce.AP.Client.Core
{
    public class UserDataStore
    {
        private int _gold;
        private int _energy;
        private int _rank;
        private long _energyUpdateTime;
        private float _bgmVolume;
        private float _sfxVolume;

        private bool _isDirty;

        public UserDataStore()
        {
            // TODO: JW: 초기 지급 값 적용
            _gold = 0;
            _energy = 0;
            _rank = 0;
            _bgmVolume = 0f;
            _sfxVolume = 0f;
            //
        }

        public void SetGold(int gold)
        {
            _gold = gold;
            _isDirty = true;
        }

        public void AddGold(int amount)
        {
            _gold += amount;
            _isDirty = true;
        }

        public int GetGold()
        {
            return _gold;
        }

        public void SetEnergy(int energy)
        {
            _energy = energy;
            _isDirty = true;
        }

        public int GetEnergy()
        {
            return _energy;
        }

        public void SetRank(int rank)
        {
            _rank = rank;
            _isDirty = true;
        }

        public int GetRank()
        {
            return _rank;
        }

        public void SetEnergyUpdateTime(long energyUpdateTime)
        {
            _energyUpdateTime = energyUpdateTime;
            _isDirty = true;
        }

        public long GetEnergyUpdateTime()
        {
            return _energyUpdateTime;
        }

        public void SetBgmVolume(float volume)
        {
            _bgmVolume = volume;
            _isDirty = true;
        }
        
        public float GetBgmVolume()
        {
            return _bgmVolume;
        }

        public void SetSfxVolume(float volume)
        {
            _sfxVolume = volume;
            _isDirty = true;
        }
        
        public float GetSfxVolume()
        {
            return _sfxVolume;
        }

        public bool IsDirty()
        {
            return _isDirty;
        }

        public void ClearDirty()
        {
            _isDirty = false;
        }
    }
}
