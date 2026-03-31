using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    public class CsvLoader
    {
        private readonly CsvReader _csvReader;
        private readonly AssetLoader _assetLoader;

        public CsvLoader(CsvReader csvReader, AssetLoader assetLoader)
        {
            _csvReader = csvReader;
            _assetLoader = assetLoader;
        }

        public async Task<List<Dictionary<string, string>>> LoadCsvByPath(string path)
        {
            var textAsset = await _assetLoader.LoadAssetByPath<TextAsset>(path);
            var rows = _csvReader.ReadCsvData(textAsset.text);
            return rows;
        }

        public async Task<List<Dictionary<string, string>>> LoadCsv(string assetID)
        {
            var textAsset = await _assetLoader.LoadAsset<TextAsset>(assetID);
            var rows = _csvReader.ReadCsvData(textAsset.text);
            return rows;
        }
    }
}
