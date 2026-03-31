using System.Linq;
using System.Threading.Tasks;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.UnityWorld.AssetData;

namespace TaskForce.AP.Client.UnityWorld
{
    public class TextStoreLoader
    {
        private readonly CsvLoader _csvLoader;

        public TextStoreLoader(CsvLoader csvLoader)
        {
            _csvLoader = csvLoader;
        }

        public async Task Load(string _languageCode, TextStore textStore)
        {
            var rows = await _csvLoader.LoadCsv(AssetID.Text);

            if (!rows.Any()) return;

            var firstRow = rows.First();
            var headers = firstRow.Keys.ToArray();
            var langHeaders = headers.Where(h => h != "key" && h != "TextID").ToArray();

            foreach (var row in rows)
            {
                if (!row.TryGetValue("key", out var key))
                    continue;

                string value = string.Empty;
                row.TryGetValue(_languageCode, out value);

                textStore.AddText(key, value);
            }
        }
    }
}
