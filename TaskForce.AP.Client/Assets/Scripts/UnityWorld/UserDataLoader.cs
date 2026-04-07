using TaskForce.AP.Client.Core;

namespace TaskForce.AP.Client.UnityWorld
{
    public class UserDataLoader : IUpdatable
    {
        private readonly UserDataStore _userDataStore;

        public UserDataLoader(UserDataStore userDataStore)
        {
            _userDataStore = userDataStore;
        }

        public void Load()
        {
            var fileService = new FileService();
            var data = fileService.LoadUserData();

            _userDataStore.SetGold(data.gold);
            _userDataStore.SetEnergy(data.energy);
            _userDataStore.SetRank(data.rank);
            _userDataStore.SetEnergyUpdateTime(data.energyUpdateTime);

            _userDataStore.ClearDirty();
        }

        public void Update()
        {
            if (!_userDataStore.IsDirty()) return;

            var fileService = new FileService();
            fileService.SaveUserData(new UserData
            {
                gold = _userDataStore.GetGold(),
                energy = _userDataStore.GetEnergy(),
                rank = _userDataStore.GetRank(),
                energyUpdateTime = _userDataStore.GetEnergyUpdateTime()
            });

            _userDataStore.ClearDirty();
        }
    }
}
