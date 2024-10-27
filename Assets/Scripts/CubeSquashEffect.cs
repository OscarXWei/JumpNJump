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
    private Renderer cubeRenderer;
    private Material explodingMaterial;

    void Start()
    {
        originalScale = transform.localScale;
        player = FindObjectOfType<PlayerController>();
        explodingMaterial = Resources.Load<Material>("Material/Exploding");
        shatteredPlayerPrefab = Resources.Load<GameObject>("Prefabs/Shatter");
        displayManager = FindObjectOfType<LevelDisplayManager>();
        cubeRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().enabled = true;
        }
        if (gameObject.CompareTag("Empty"))
        {
            rb.isKinematic = true;
            if (GetComponent<Collider>() != null)
            {
                GetComponent<Collider>().enabled = false;
            }
            return;
        }
        if (player != null && player.transform.position.y - player.transform.localScale.y / 2 > transform.position.y + transform.localScale.y / 2)
        {
            float distanceToPlayer = Mathf.Sqrt(Mathf.Pow(player.transform.position.x - transform.position.x, 2) + Mathf.Pow(player.transform.position.z - transform.position.z, 2));
            if (distanceToPlayer < 1f && player.isCharging)
            {
                ApplySquashEffect(player.currentJumpForce / player.maxJumpForce);
            }
            else if (distanceToPlayer < 0.5f && player.isSimpleRolling)
            {
                player.turnOffPhysics();
            }
            else
            {
                ResetSquashEffect();
            }

            if (distanceToPlayer < 0.5f)
            {
                //Debug.Log(gameObject.tag);
                if (gameObject.CompareTag("Explosive")) // Assuming your player has the "Player" tag
                {
                    Debug.Log("Explodeing!");
                    StartCoroutine(DestroyCubeAfterDelay(3f));
                }

                // else if (gameObject.CompareTag("SpringStart")) // Assuming your player has the "Player" tag
                // {
                //     var result = displayManager.findMatchedPlatform(gameObject);
                //     if (result.success)
                //         player.setTargetCubeJumping(result.val);
                //     //sss
                // }
                else if (gameObject.CompareTag("EnemySrc")) // Assuming your player has the "Player" tag
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
                else if (gameObject.CompareTag("SpringsTrigger") && player.transform.position.y - transform.position.y < 0.2f + player.transform.localScale.y / 2 + transform.localScale.y / 2)
                // Assuming your player has the "Player" tag
                {
                    var results = displayManager.findTemp(gameObject);
                    if (results.success)
                        if (!hasGeneratedObject)
                            foreach (var obj in results.vals)
                                obj.SendMessage("setTempSprings");

                    hasGeneratedObject = true;
                    //sss 
                }
                else if (gameObject.CompareTag("HiddenTrigger") && player.transform.position.y - transform.position.y < 0.2f + player.transform.localScale.y / 2 + transform.localScale.y / 2) // Assuming your player has the "Player" tag
                {
                    var results = displayManager.findTemp(gameObject);
                    if (results.success)
                        //if (!hasGeneratedObject)
                        foreach (var obj in results.vals)
                            obj.SendMessage("setTempHiddens");

                    hasGeneratedObject = true;
                    //sss
                }
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
        // Debug.Log(gameObject.tag);
        // if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Explosive")) // Assuming your player has the "Player" tag
        // {
        //     Debug.Log("Explodeing!");
        //     StartCoroutine(DestroyCubeAfterDelay(3f));
        // }

        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("SpringStart")) // Assuming your player has the "Player" tag
        {
            var result = displayManager.findMatchedPlatform(gameObject);
            if (result.success)
                player.setTargetCubeJumping(result.val);
            //sss
        }
        // else if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("EnemySrc")) // Assuming your player has the "Player" tag
        // {
        //     var result = displayManager.findMatchedPlatform(gameObject);
        //     var resultLevel = displayManager.findPlatformEnemyType(gameObject);
        //     var resultRewardLevel = displayManager.findPlatformEnemyType(gameObject);
        //     if (result.success && resultLevel.success && resultRewardLevel.success)
        //         if (!hasGeneratedObject)
        //             displayManager.generateEnemy(resultLevel.val, resultRewardLevel.val, result.val.transform.localPosition);
        //     hasGeneratedObject = true;
        //     //sss
        // }
        // else if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("SpringsTrigger")) // Assuming your player has the "Player" tag
        // {
        //     var results = displayManager.findTempSprings(gameObject);
        //     if (results.success)
        //         if (!hasGeneratedObject)
        //             foreach (var obj in results.vals)
        //                 obj.SendMessage("setTempSprings");

        //     hasGeneratedObject = true;
        //     //sss
        // }



        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Explosive")) // Assuming your player has the "Player" tag
        {
            cubeRenderer.material = explodingMaterial;
            StartCoroutine(DestroyCubeAfterDelay(5f));
        }
    }

    public void setEnemyType(int type)
    {
        enemyType = type;
    }

    public void setEnemyRewardType(int type)
    {
        enemyRewardType = type;
    }

    public void setTempSprings()
    {
        Debug.Log("setting spring now");
        Renderer objectRenderer = GetComponent<Renderer>();

        // Setup material for transparency
        Material material = objectRenderer.material;
        material.SetFloat("_Mode", 3);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;

        StopAllCoroutines();
        StartCoroutine(TempSpringRoutine());
    }

    private IEnumerator TempSpringRoutine()
    {
        // Change to spring
        Renderer objectRenderer = GetComponent<Renderer>();
        gameObject.tag = "SpringStart";
        objectRenderer.material.color = Color.green;

        // Wait for 30 seconds
        yield return new WaitForSeconds(10f);

        // Change back
        gameObject.tag = "Empty";
        objectRenderer.material.color = Color.clear; // Clear color
    }

    public void setTempHiddens()
    {
        Renderer objectRenderer = GetComponent<Renderer>();

        // Setup material for transparency
        Material material = objectRenderer.material;
        material.SetFloat("_Mode", 3);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;

        StopAllCoroutines();
        StartCoroutine(TempHiddenRoutine());
    }

    private IEnumerator TempHiddenRoutine()
    {
        // Change to spring
        Renderer objectRenderer = GetComponent<Renderer>();
        gameObject.tag = "Platform";
        objectRenderer.material.color = Color.blue;

        yield return new WaitForSeconds(5f);

        // Change back
        gameObject.tag = "Hidden";
        objectRenderer.material.color = Color.clear; // Clear color
    }



}
