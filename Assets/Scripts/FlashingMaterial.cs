using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingMaterial : MonoBehaviour
{
    public Material flashingMaterial;    // The material to flash (drag it in the Inspector)
    public Color flashColor = Color.red; // Color to flash to
    public float flashSpeed = 0.1f;      // Speed of flashing
    public float duration = 12f;         // Duration of the flashing effect

    private Material originalMaterial;   // Store the original material
    private Renderer playerRenderer;     // Renderer of the player

    void Start()
    {
        playerRenderer = GetComponent<Renderer>();
        originalMaterial = playerRenderer.material; // Store the original material
    }

    public void StartFlashingEffect()
    {
        StartCoroutine(FlashingEffect(duration));
    }

    private IEnumerator FlashingEffect(float duration)
    {
        float timeElapsed = 0f;
        bool isFlashing = false;

        while (timeElapsed < duration)
        {
            // Alternate between original material and flash material
            if (isFlashing)
            {
                playerRenderer.material.color = flashColor;
            }
            else
            {
                playerRenderer.material.color = originalMaterial.color;
            }

            isFlashing = !isFlashing; // Toggle the flashing state
            timeElapsed += flashSpeed;
            yield return new WaitForSeconds(flashSpeed);
        }

        // Restore the original material when the flashing effect is over
        playerRenderer.material.color = originalMaterial.color;
    }
}