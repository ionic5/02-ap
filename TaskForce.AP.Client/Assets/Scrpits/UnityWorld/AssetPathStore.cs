using System.Collections.Generic;
using TaskForce.AP.Client.Core;

namespace TaskForce.AP.Client.UnityWorld
{
    public class AssetPathStore
    {
        private readonly Dictionary<string, string> _assetPaths;
        private readonly Dictionary<string, string> _fontPaths;
        private readonly ILogger _logger;

        public AssetPathStore(ILogger logger)
        {
            _assetPaths = new Dictionary<string, string>();
            _fontPaths = new Dictionary<string, string>();
            _logger = logger;
        }

        public string GetAssetPath(string assetID)
        {
            if (string.IsNullOrEmpty(assetID))
            {
                _logger.Warn($"Empty asset id used.");
                return null;
            }

            if (_assetPaths.ContainsKey(assetID))
                return _assetPaths[assetID];

            _logger.Warn($"Failed to find asset path for {assetID}");
            return string.Empty;
        }

        public string GetAssetPathByLanguageCode(string languageCode)
        {
            if (_fontPaths.ContainsKey(languageCode))
                return _fontPaths[languageCode];

            return string.Empty;
        }

        public void AddAssetPath(string assetID, string path)
        {
            _assetPaths.Add(assetID, path);
        }

        public void AddFontPath(string languageCode, string path)
        {
            _fontPaths.Add(languageCode, path);
        }

        public void Clear()
        {
            _assetPaths.Clear();
            _fontPaths.Clear();
        }
    }
}
