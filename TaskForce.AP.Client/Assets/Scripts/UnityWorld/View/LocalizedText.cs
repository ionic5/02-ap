using TaskForce.AP.Client.Core;
using TMPro;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View
{
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        private string _key;

        public void Initialize(TextStore textStore)
        {
            _key = _text.text;
            _text.text = textStore.GetText(_key);
        }
    }
}
