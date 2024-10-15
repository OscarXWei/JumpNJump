using UnityEngine;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public JumpPowerUI jumpPowerUI;
    private PlatformManager platformManager;
    private GameOverUI gameOverUI;

    [Header("Jump Settings")]
    public float maxJumpForce = 10f;
    public float chargeRate = 5f;
    public float jumpAngle = 45f;

    [Header("Optimal Jump Range")]
    [Range(0.05f, 0.2f)]
    public float optimalRangePercentage = 0.1f;

    [Header("Adjustments")]
    public float forceAdjustment = 1f;
    public bool debugMode = false;

    [Header("Shatter Effect")]
    public GameObject shatteredPlayerPrefab;
    public float shatterDelay = 0.5f;
    public float resetDelay = 2f;

    [Header("Squash Effect")]
    public float maxSquashAmount = 0.2f;
    public float squashSpeed = 2f;

    public event Action OnJumpSuccess;
    public event Action OnGameOver;
    public bool isGameOver = false;
    public float gameOverDelay = 1f;

    public float currentJumpForce = 0f;
    public bool isCharging = false;
    private bool isJumping = false;

    private Rigidbody rb;
    private GameObject nextCube;
    private GameObject currentCube;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 originalScale;
    private Color currentPlatformColor;

    [Header("Rolling Effect")]
    public float rollSpeed = 3600f; // 每秒旋转的角度
    private Vector3 rollAxis;
    private float totalRotation = 0f;
    private bool isRolling = false;
    private float targetRotation = 3600f; // 完整的一周旋转

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public float shootCooldown = 0.5f;

    private float lastShootTime;
    private float maxHealth = 100.0f;
    private float currentHealth = 100.0f;
    [SerializeField] HealthBarController healthBar;

    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthBarController>();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        healthBar.UpdateHp(currentHealth, maxHealth);
        platformManager = FindObjectOfType<PlatformManager>();
        gameOverUI = FindObjectOfType<GameOverUI>();

        if (platformManager == null)
        {
            Debug.LogError("PlatformManager not found in the scene!");
        }

        if (gameOverUI == null)
        {
            Debug.LogError("GameOverUI not found in the scene!");
        }

        UpdateCubeReferences();
        jumpPowerUI.SetMaxPower(maxJumpForce);
        startPosition = transform.position;
        startRotation = transform.rotation;
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (!isJumping && !isGameOver && GameManager.Instance.isStarting)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCharging();
            }

            if (Input.GetKey(KeyCode.Space) && isCharging)
            {
                ContinueCharging();
                ApplySquashEffect();
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (debugMode) Debug.Log("Space key released, attempting to jump");
                Jump();
                ResetSquashEffect();
            }
        }
        else if (isJumping && isRolling)
        {
            // 在跳跃过程中应用翻滚效果
            Debug.Log("Attempting to apply rolling effect");
            ApplyRollingEffect();
        }

        // if (Input.GetMouseButtonDown(0) && Time.time - lastShootTime > shootCooldown && !isGameOver)
        // {
        //     Shoot();
        // }
    }
    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position + transform.forward, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
        lastShootTime = Time.time;
        // SoundManager.Instance.PlayShootSound(); // 假设您有这个方法
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.UpdateHp(currentHealth, maxHealth);
    }

    void UpdateCubeReferences()
    {
        currentCube = platformManager.GetCurrentPlatform();
        nextCube = platformManager.GetNextPlatform();
        if (nextCube != null)
        {
            CalculateOptimalJumpForce();
        }
    }

    void ApplySquashEffect()
    {
        float squashAmount = Mathf.Lerp(0, maxSquashAmount, currentJumpForce / maxJumpForce);
        Vector3 newScale = new Vector3(
            originalScale.x * (1 + squashAmount),
            originalScale.y * (1 - squashAmount),
            originalScale.z * (1 + squashAmount)
        );
        transform.localScale = Vector3.Lerp(transform.localScale, newScale, Time.deltaTime * squashSpeed);
    }

    void ResetSquashEffect()
    {
        transform.localScale = originalScale;
    }

    void StartCharging()
    {
        isCharging = true;
        currentJumpForce = 0f;
        CalculateOptimalJumpForce();
    }

    void ContinueCharging()
    {
        currentJumpForce += chargeRate * Time.deltaTime;
        currentJumpForce = Mathf.Min(currentJumpForce, maxJumpForce);
        jumpPowerUI.SetPower(currentJumpForce);
        //if (debugMode) Debug.Log($"Current jump force: {currentJumpForce}");
    }

    void ApplyRollingEffect()
    {
        if (totalRotation < targetRotation)
        {
            float rotationThisFrame = rollSpeed * Time.deltaTime;
            transform.Rotate(rollAxis, rotationThisFrame, Space.World);
            totalRotation += rotationThisFrame;
            Debug.Log($"Applying rotation. Total rotation: {totalRotation}, Roll axis: {rollAxis}"); // 调试日志
        }
        else
        {
            isRolling = false;
            // 确保完成后角度是精确的
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            Debug.Log("Rotation completed");
        }
    }

    void Jump()
    {
        if (isCharging && nextCube != null)
        {
            Vector3 jumpDirection = CalculateJumpDirection();
            rb.AddForce(jumpDirection * currentJumpForce, ForceMode.Impulse);
            isCharging = false;
            isJumping = true;
            isRolling = true;
            totalRotation = 0f;

            // 计算翻滚轴（与跳跃方向和上向量的叉积）
            Vector3 horizontalJumpDir = new Vector3(jumpDirection.x, 0, jumpDirection.z).normalized;
            rollAxis = Vector3.Cross(horizontalJumpDir, Vector3.up).normalized;

            // 根据跳跃方向决定是前翻还是后翻
            if (Vector3.Dot(transform.forward, horizontalJumpDir) >= 0)
            {
                rollAxis = -rollAxis; // 前翻
            }
            // 如果 Vector3.Dot < 0，保持 rollAxis 不变，这将是后翻

            jumpPowerUI.SetPower(0);

            //SoundManager.Instance.PlayJumpSound();  
        }
        else
        {
            if (debugMode)
            {
                Debug.Log($"Failed to jump. isCharging: {isCharging}, nextCube: {(nextCube != null ? "not null" : "null")}");
                Debug.Log($"Current player position: {transform.position}");
                if (nextCube != null)
                {
                    Debug.Log($"Next cube position: {nextCube.transform.position}");
                }
            }
        }
    }

    Vector3 CalculateJumpDirection()
    {
        Vector3 targetDirection = nextCube.transform.position - transform.position;
        float horizontalDistance = new Vector3(targetDirection.x, 0, targetDirection.z).magnitude;
        float radianAngle = jumpAngle * Mathf.Deg2Rad;

        float cosAngle = Mathf.Cos(radianAngle);
        float tanAngle = Mathf.Tan(radianAngle);

        Vector3 horizontalDir = new Vector3(targetDirection.x, 0, targetDirection.z).normalized;
        Vector3 jumpDirection = horizontalDir * cosAngle + Vector3.up * tanAngle;

        return jumpDirection.normalized;
    }

    void CalculateOptimalJumpForce()
    {
        if (nextCube != null)
        {
            Vector3 targetDirection = nextCube.transform.position - transform.position;
            float horizontalDistance = new Vector3(targetDirection.x, 0, targetDirection.z).magnitude;
            float verticalDistance = targetDirection.y;
            float gravity = Physics.gravity.magnitude;
            float radianAngle = jumpAngle * Mathf.Deg2Rad;

            float v0Squared = (gravity * horizontalDistance * horizontalDistance) /
                              (2 * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle) *
                               (horizontalDistance * Mathf.Tan(radianAngle) - verticalDistance));
            float v0 = Mathf.Sqrt(v0Squared);

            float exactJumpForce = v0 * rb.mass * forceAdjustment;
            float optimalJumpForce = Mathf.Min(exactJumpForce, maxJumpForce);

            float rangeHalf = optimalJumpForce * optimalRangePercentage / 2;
            float minOptimal = Mathf.Max(optimalJumpForce - rangeHalf, 0);
            float maxOptimal = Mathf.Min(optimalJumpForce + rangeHalf, maxJumpForce);

            jumpPowerUI.SetOptimalPowerRange(minOptimal, maxOptimal);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Collision detection bugs
        GameObject hitPlatform = collision.gameObject;
        Vector3 platformPosition = hitPlatform.transform.position;

        if (hitPlatform == platformManager.GetNextPlatform() && transform.position.y > hitPlatform.transform.position.y + 0.6)        
        {
            SucceedJump(hitPlatform);
        }
        else if (hitPlatform.CompareTag("Terrain"))
        {
            FailJump();
        }

        
        isRolling = false;
  
        // transform.rotation = Quaternion.identity;
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    private void SucceedJump(GameObject hitPlatform)
    {
        isJumping = false;
        platformManager.MoveToNextPlatform();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = hitPlatform.transform.position + Vector3.up * 0.5f;
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    private IEnumerator CheckPlatformRules()
    {
        if (ColorEquals(currentPlatformColor, Color.red))
        {
            ScoreManager.Instance.AddScore(1);
        }
        else if (ColorEquals(currentPlatformColor, Color.blue))
        {
            ScoreManager.Instance.AddScore(2);
        }
        else if (ColorEquals(currentPlatformColor, Color.yellow))
        {
            int randomScore = UnityEngine.Random.Range(0, 4); // 随机分数 0-3
            ScoreManager.Instance.AddScore(randomScore);
        }
        else if (ColorEquals(currentPlatformColor, Color.cyan))
        {
            ScoreManager.Instance.AddScore(3);
        }
        else if (ColorEquals(currentPlatformColor, Color.green))
        {
            float startTime = Time.time;
            while (Time.time - startTime < 2f)
            {
                if (!IsPlayerOnCurrentPlatform())
                {
                    yield break;
                }
                yield return null;
            }
            if (IsPlayerOnCurrentPlatform())
            {
                ScoreManager.Instance.AddScore(3);
            }
        }
        else if (ColorEquals(currentPlatformColor, Color.black))
        {
            float startTime = Time.time;
            while (Time.time - startTime < 2f)
            {
                if (!IsPlayerOnCurrentPlatform())
                {
                    yield break;
                }
                yield return null;
            }
            ExplodePlatform();
        }
    }

    private bool IsPlayerOnCurrentPlatform()
    {
        return IsPlayerOnCube(platformManager.GetCurrentPlatform());
    }

    private bool ColorEquals(Color a, Color b)
    {
        return Mathf.Approximately(a.r, b.r) &&
               Mathf.Approximately(a.g, b.g) &&
               Mathf.Approximately(a.b, b.b) &&
               Mathf.Approximately(a.a, b.a);
    }

    private void ExplodePlatform()
    {
        Debug.Log("Black platform exploded!");
        FailJump();
        GameManager.Instance.GameOver("You stayed too long on the black platform!");
    }

    private void RetryJump()
    {
        isJumping = false;
        if (debugMode) Debug.Log("Landed back on the current cube. You can try jumping again.");
    }

    private bool IsPlayerOnCube(GameObject cube)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
        {
            return hit.collider.gameObject == cube;
        }
        return false;
    }

    private void FailJump()
    {
        isJumping = false;
        isGameOver = true;
        Invoke("ShatterPlayer", shatterDelay);
        //SoundManager.Instance.PlayShatterSound();
        StartCoroutine(ShowGameOverAfterDelay());
    }

    private IEnumerator ShowGameOverAfterDelay()
    {
        yield return new WaitForSeconds(gameOverDelay);

        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver();
        }
        else
        {
            Debug.LogError("GameOverUI is not set!");
        }
    }

    void ShatterPlayer()
    {
        isJumping = false;
        Vector3 shatterPosition = transform.position;

        if (shatteredPlayerPrefab != null)
        {
            GameObject shatteredPlayer = Instantiate(shatteredPlayerPrefab, shatterPosition, Quaternion.identity);
            Debug.Log($"Shattered player instantiated at position: {shatteredPlayer.transform.position}");
        }

        Invoke("HidePlayer", 0.1f);

        OnGameOver?.Invoke();
    }

    void HidePlayer()
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
    }

    public void ResetGameState()
    {
        if (debugMode) Debug.Log("Resetting player game state...");

        if (platformManager != null && platformManager.GetCurrentPlatform() != null)
        {
            transform.position = platformManager.GetCurrentPlatform().transform.position + Vector3.up;
            if (debugMode) Debug.Log($"Reset player position to: {transform.position}");
        }
        else
        {
            transform.position = startPosition;
            if (debugMode) Debug.Log($"Reset player position to start position: {startPosition}");
        }
        transform.rotation = startRotation;

        isGameOver = false;
        isJumping = false;
        isCharging = false;
        currentJumpForce = 0f;

        StopAllCoroutines();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (jumpPowerUI != null)
        {
            jumpPowerUI.SetPower(0f);
        }

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null) renderer.enabled = true;

        Collider collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = true;

        UpdateCubeReferences();

        if (debugMode)
        {
            Debug.Log("Player game state reset completed");
            Debug.Log($"Current cube: {(currentCube != null ? currentCube.name : "null")}");
            Debug.Log($"Next cube: {(nextCube != null ? nextCube.name : "null")}");
        }
    }

    public void TryAgain()
    {

        // 重置玩家位置到目标平台
        transform.position = currentCube.transform.position + Vector3.up;
        transform.rotation = startRotation;

        isGameOver = false;
        StopAllCoroutines();

        // 重置玩家状态
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isJumping = false;
        currentJumpForce = 0f;
        jumpPowerUI.SetPower(0f);

        // 显示玩家
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;

        ScoreManager.Instance.RetryScore();
        if (debugMode) Debug.Log("Player reset for Try Again");
    }
}