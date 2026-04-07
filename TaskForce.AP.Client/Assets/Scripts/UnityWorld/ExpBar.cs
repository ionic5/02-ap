using UnityEngine;
using UnityEngine.UI;

namespace TaskForce.AP.Client.UnityWorld
{
    public class ExpBar : MonoBehaviour
    {
        private int _value;
        private int _maxValue;

        [SerializeField]
        private Slider _slider;

        public void SetValue(int value)
        {
            _value = value;
            UpdateGaugeBar();
        }

        public void SetMaxValue(int value)
        {
            _maxValue = value;
            UpdateGaugeBar();
        }

        public void SetVisible(bool v)
        {
            gameObject.SetActive(v);
        }

        public void UpdateGaugeBar()
        {
            if (_maxValue <= 0)
                return;

            if (_value < 0)
                _value = 0;

            _slider.value = _value / (float)_maxValue;
        }
    }
}
