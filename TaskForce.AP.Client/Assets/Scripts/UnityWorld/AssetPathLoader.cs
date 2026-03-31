using System.Threading.Tasks;

namespace TaskForce.AP.Client.UnityWorld
{
    public class AssetPathLoader
    {
        private readonly CsvLoader _csvLoader;

        public AssetPathLoader(CsvLoader csvLoader)
        {
            _csvLoader = csvLoader;
        }

        public async Task Load(AssetPathStore assetPathStore)
        {
            await LoadAssetPath(assetPathStore);
        }

        private async Task LoadAssetPath(AssetPathStore assetPathStore)
        {
            var rows = await _csvLoader.LoadCsvByPath("Assets/Addressables/AssetData/Asset.csv");
            foreach (var row in rows)
                assetPathStore.AddAssetPath(row["assetID"], row["path"]);
        }
    }
}
