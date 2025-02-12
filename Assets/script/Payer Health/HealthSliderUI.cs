using UnityEngine;
using UnityEngine.UI;

public class HealthSliderUI : MonoBehaviour
{
    public Slider healthSlider;  // Référence au Slider

    public void Initialize(float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }

    public void UpdateHealth(float currentHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }
}
