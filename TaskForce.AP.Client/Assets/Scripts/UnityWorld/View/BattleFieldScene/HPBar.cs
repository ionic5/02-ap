using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Image gauge;
    Material hpMaterial;

    public void SetHPGauge(float hp)
    {
        hpMaterial = gauge.material; 
        hpMaterial.SetFloat("_Fill", hp);
        
        gauge.fillAmount = hp;
    }
}
