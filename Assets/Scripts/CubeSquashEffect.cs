using System.Collections;
using UnityEngine;

public class CubeSquashEffect : MonoBehaviour
{
    public float maxSquashAmount = 0.2f;
    public float squashSpeed = 2f;
    private Vector3 originalScale;
    private PlayerController player;
    public GameObject shatteredPlayerPrefab;
    private LevelDisplayManager displayManager;

    void Start()
    {
        originalScale = transform.localScale;
        player = FindObjectOfType<PlayerController>();
        shatteredPlayerPrefab = Resources.Load<GameObject>("Prefabs/Shatter");
        displayManager = FindObjectOfType<LevelDisplayManager>();
    }

    void Update()
    {
        if (player != null && player.transform.position.y > transform.position.y)
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (distanceToPlayer < 1f && player.isCharging)
            {
                ApplySquashEffect(player.currentJumpForce / player.maxJumpForce);
            }
            else if (distanceToPlayer < 1f && player.isSimpleRolling)
            {
                player.turnOffPhysics();
            }
            else
            {
                ResetSquashEffect();
            }
        }
        else
        {
            ResetSquashEffect();
        }
    }

    private IEnumerator DestroyCubeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject shatteredPlayer = Instantiate(shatteredPlayerPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject); // Destroy the cube
    }

    void ApplySquashEffect(float chargePercentage)
    {
        float squashAmount = Mathf.Lerp(0, maxSquashAmount, chargePercentage);
        Vector3 newScale = new Vector3(
            originalScale.x * (1 + squashAmount),
            originalScale.y * (1 - squashAmount),
            originalScale.z * (1 + squashAmount)
        );
        //player.turnOffHorizontalPhysics();
        transform.localScale = Vector3.Lerp(transform.localScale, newScale, Time.deltaTime * squashSpeed);
    }

    void ResetSquashEffect()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * squashSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Explosive")) // Assuming your player has the "Player" tag
        {
            Debug.Log("Explodeing!");
            StartCoroutine(DestroyCubeAfterDelay(3f));
        }

        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("SpringStart")) // Assuming your player has the "Player" tag
        {
            var result = displayManager.findMatchedPlatform(gameObject);
            if (result.success)
                player.setTargetCubeJumping(result.val);
            //sss
        }



        //if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Platform")) // Assuming your player has the "Player" tag
        //{
        //    StartCoroutine(DestroyCubeAfterDelay(5f));
        //}
    }



}
