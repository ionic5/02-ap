using System.Collections.Generic;

namespace TaskForce.AP.Client.Core
{
    public class TextStore
    {
        private readonly Core.ILogger _logger;
        private readonly Dictionary<string, string> _texts;

        public TextStore(Core.ILogger logger)
        {
            _logger = logger;
            _texts = new Dictionary<string, string>();
        }

        public string GetText(string textID)
        {
            if (string.IsNullOrEmpty(textID))
            {
                _logger.Warn($"'{textID}' is null or empty.");
                return string.Empty;
            }

            if (_texts.TryGetValue(textID, out var value))
                return value;
            _logger.Warn($"TextID '{textID}' not found.");
            return textID;
        }

        public void Clear()
        {
            _texts.Clear();
        }

        public void AddText(string key, string value)
        {
            _texts[key] = value;
        }
    }
}
