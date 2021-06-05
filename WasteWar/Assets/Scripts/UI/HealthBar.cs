using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image foregroundImage;
    public HealthComponent Health;

    void Start()
    {
        foregroundImage.fillAmount = 0.5f;
    }
    void Update()
    {
        print(GetComponentInParent<HealthComponent>().Health);
    }
}
