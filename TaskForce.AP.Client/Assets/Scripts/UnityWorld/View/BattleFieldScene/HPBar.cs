using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Slider gauge;

    public void SetHPGauge(float hp)
    {
        gauge.value = hp;
    }
}
