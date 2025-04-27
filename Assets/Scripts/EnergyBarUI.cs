using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    public Image fillImage; // The fill element of the bar
    public PlayerController player; // The script that tracks energy

    void Update()
    {
        if (player == null || fillImage == null) return;

        // Get energy % (0.0 to 1.0)
        float energyPercent = player.currentEnergy / player.maxEnergy;

        // Smoothly interpolate the fill amount
        fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, energyPercent, Time.deltaTime * 10f);
    }
}
