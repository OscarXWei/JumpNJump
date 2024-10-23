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
    private bool hasGeneratedObject = false;
    
    private int enemyType = 0;
    private int enemyRewardType = 0;

    void Start()
    {
        originalScale = transform.localScale;
        player = FindObjectOfType<PlayerController>();
        shatteredPlayerPrefab = Resources.Load<GameObject>("Prefabs/Shatter");
        displayManager = FindObjectOfType<LevelDisplayManager>();
    }

    void Update()
    {
        if (player != null && player.transform.position.y - player.transform.localScale.y/2 > transform.position.y + transform.localScale.y/2)
        {
            float distanceToPlayer = Mathf.Sqrt(Mathf.Pow(player.transform.position.x - transform.position.x, 2) + Mathf.Pow(player.transform.position.z - transform.position.z, 2));
            if (distanceToPlayer < 1f && player.isCharging)
            {
                ApplySquashEffect(player.currentJumpForce / player.maxJumpForce);
            }
            else if (player.isSimpleRolling && distanceToPlayer < 0.5f)
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
        Debug.Log(gameObject.tag);
        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Explosive")) // Assuming your player has the "Player" tag
        {
            Debug.Log("Explodeing!");
            StartCoroutine(DestroyCubeAfterDelay(3f));
        }

        else if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("SpringStart")) // Assuming your player has the "Player" tag
        {
            Debug.Log("should do spring");
            var result = displayManager.findMatchedPlatform(gameObject);
            if (result.success)
                player.setTargetCubeJumping(result.val);
            //sss
        }
        else if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("EnemySrc")) // Assuming your player has the "Player" tag
        {
            var result = displayManager.findMatchedPlatform(gameObject);
            var resultLevel = displayManager.findPlatformEnemyType(gameObject);
            var resultRewardLevel = displayManager.findPlatformEnemyType(gameObject);
            if (result.success && resultLevel.success && resultRewardLevel.success)
            	if (!hasGeneratedObject)
                	displayManager.generateEnemy(resultLevel.val, resultRewardLevel.val, result.val.transform.localPosition);
            hasGeneratedObject = true;
            //sss
        }



        //if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Platform")) // Assuming your player has the "Player" tag
        //{
        //    StartCoroutine(DestroyCubeAfterDelay(5f));
        //}
    }
    
    public void setEnemyType(int type)
    {
    	enemyType = type;
    }
    
    public void setEnemyRewardType(int type)
    {
    	enemyRewardType = type;
    }



}
