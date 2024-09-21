using UnityEngine;
using UnityEngine.UI;

public class JumpPowerUI : MonoBehaviour
{
    public Slider powerSlider;
    public Image fillArea;
    public Color lowPowerColor = Color.blue;
    public Color optimalPowerColor = Color.yellow;
    public Color highPowerColor = Color.red;
    
    private float optimalMinPower = 0f;
    private float optimalMaxPower = 1f;
    private float currentPower = 0f;
    private float maxPower = 1f;

    public bool debugMode = false;

    public void SetMaxPower(float max)
    {
        maxPower = max;
        powerSlider.maxValue = max;
        if (debugMode) Debug.Log($"Max power set to: {max}");
    }

    public void SetPower(float power)
    {
        currentPower = power;
        powerSlider.value = power;
        UpdateFillColor();
        // if (debugMode) Debug.Log($"Current power: {power}, Optimal range: {optimalMinPower} - {optimalMaxPower}");
    }

    public void SetOptimalPowerRange(float minPower, float maxPower)
    {
        optimalMinPower = Mathf.Clamp(minPower, 0, maxPower);
        optimalMaxPower = Mathf.Clamp(maxPower, minPower, maxPower);
        if (debugMode) Debug.Log($"Optimal power range set to: {optimalMinPower} - {optimalMaxPower}");
    }

    private void UpdateFillColor()
    {
        Color newColor;

        if (currentPower < optimalMinPower)
        {
            newColor = lowPowerColor;
            if (debugMode) Debug.Log("Power in low (blue) range");
        }
        else if (currentPower <= optimalMaxPower)
        {
            newColor = optimalPowerColor;
            if (debugMode) Debug.Log("Power in optimal (yellow) range");
        }
        else
        {
            newColor = highPowerColor;
            if (debugMode) Debug.Log("Power in high (red) range");
        }

        fillArea.color = newColor;
        if (debugMode) Debug.Log($"Set fill color to: R:{newColor.r:F2}, G:{newColor.g:F2}, B:{newColor.b:F2}");
    }
}