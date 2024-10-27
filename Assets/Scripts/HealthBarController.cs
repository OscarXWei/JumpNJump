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

    public void UpdateHp(int currentHealth, int maxHealth)
    {
        slider.value = (float)currentHealth / (float)maxHealth;
    }
}
