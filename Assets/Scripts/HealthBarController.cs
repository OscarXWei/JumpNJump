using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [Header("Health Bar Settings")]
    [SerializeField] private Slider slider;
    public Transform healthBar;
    private Camera mainCamera;


    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 lookDirection = mainCamera.transform.position - healthBar.position;
        lookDirection.y = 0;
        healthBar.rotation = Quaternion.LookRotation(lookDirection);
    }

    public void UpdateHp(float currentHealth, float maxHealth)
    {
        slider.value = currentHealth / maxHealth;
    }
}
