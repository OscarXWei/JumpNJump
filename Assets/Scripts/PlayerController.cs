using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using System.IO.Compression;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public JumpPowerUI jumpPowerUI;
    //private PlatformManager platformManager;
    private GameOverUI gameOverUI;

    [Header("Jump Settings")]
    public float maxJumpForce = 8f;
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
    private float totalRotation = 1f;
    private bool isRolling = false;
    private float targetRotation = 3600f; // 完整的一周旋转
    // private float rollTimer = 0f;

    [Header("Simple Rolling")]
    public float simpleRollSpeed = 3636f; // 每秒旋转的角度
    public float simpleRollDuration = 1f;
    private Vector3 simpleRollDirection;
    public bool isSimpleRolling = false;
    private float simpleRollTimer = 0f;
    private float simpleRollHorizontal = 1f;
    private float simpleRollVertical = 0f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public float shootCooldown = 0.5f;

    private float lastShootTime;
    private float maxHealth = 100.0f;
    private float currentHealth = 100.0f;
    [SerializeField] HealthBarController healthBar;

    [Header("Level Data")]
    public LevelDisplayManager levelDisplayManager;
    private LevelData currentLevelData;
    private Transform startPlatform;
    private Transform goalPlatform;

    [Header("UI Direction Arrow")]
    public Image directionArrowImage;
    public Sprite customArrowSprite;
    public float arrowRotationSpeed = 10f;
    public Color arrowColor = Color.white;
    public float arrowSize = 50f;
    private bool isShowingArrow = true;

    private Transform currentPlatform;
    private Vector3 lastPlatformPosition;
    private Vector3 platformVelocity;

    [Header("Target Cube")]
    private bool nowHasTargetCube = false;
    private GameObject targetCube;

    [Header("Elongate Effect")]
    private int maxElongateLevel = 5;
    private int currentElongateLevel = 1;
    private float baseElongateMultiplier = 1f;
    private float elongateDuration = 20f;
    public float scrollSensitivity = 1f;
    private bool isElongated = false;
    private Coroutine elongateCoroutine;


    public Material invincibleMaterial;
    public Material originalMaterial;
    private Renderer playerRenderer;
    public int score;
    private bool isInvincible = false;
    public float remainingTime = 12f; // Set the time limit for the object to exist
    public TextMeshProUGUI timerText;
    private bool isLevelCompleting = false;

    private bool isFailed = false;
    private int health = 5;
    private GameObject recoveredPlatform;

    public PlayerController Instance { get; }


    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthBarController>();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        healthBar.UpdateHp(health, 5);
        gameOverUI = FindObjectOfType<GameOverUI>();
        levelDisplayManager = FindObjectOfType<LevelDisplayManager>();
        playerRenderer = GetComponent<Renderer>();

        jumpPowerUI.SetMaxPower(maxJumpForce);
        originalScale = transform.localScale;
        //currentElongation = elongateFactor;

        SetupLevel();
        SetupArrow();

        // 初始化平台相关变量
        currentPlatform = null;
        lastPlatformPosition = Vector3.zero;
        platformVelocity = Vector3.zero;
    }

    void SetupArrow()
    {

        directionArrowImage.color = arrowColor;

        directionArrowImage.sprite = customArrowSprite;

        RectTransform rectTransform = directionArrowImage.rectTransform;
        rectTransform.sizeDelta = new Vector2(arrowSize, arrowSize);

        directionArrowImage.transform.rotation = Quaternion.identity;

        directionArrowImage.gameObject.SetActive(false);
    }

    public void SetupLevel()
    {
        currentLevelData = levelDisplayManager.GetCurrentLevelData();
        SetStartAndGoalPlatforms();
        SetPlayerToStartPosition();
    }

    void SetStartAndGoalPlatforms()
    {
        if (currentLevelData != null && currentLevelData.platforms != null)
        {
            var startPlatformData = currentLevelData.platforms.Find(p => p.type == LevelData.PlatformType.Start);
            var goalPlatformData = currentLevelData.platforms.Find(p => p.type == LevelData.PlatformType.Goal);

            if (startPlatformData != null)
            {
                startPlatform = FindPlatformTransform(startPlatformData.position);
            }

            if (goalPlatformData != null)
            {
                goalPlatform = FindPlatformTransform(goalPlatformData.position);
            }
        }
    }

    Transform FindPlatformTransform(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.1f);
        return colliders.Length > 0 ? colliders[0].transform : null;
    }

    void SetPlayerToStartPosition()
    {
        if (startPlatform != null)
        {
            transform.position = startPlatform.position + Vector3.up * 0.5f;
            startPosition = transform.position;
            startRotation = transform.rotation;
        }
        else
        {
            Debug.LogError("Start platform not found!");
        }
    }

    void Update()
    {
        if (transform.position.y < 1f && !isFailed)
        {

            FailJump();

            return;  // 返回以避免执行其他更新逻辑
        }

        //Debug.Log("is in update");
        //Debug.Log($"isJumping {isJumping}, isGameOver {isGameOver}, isStarting {GameManager.Instance.isStarting}");
        if (!isJumping && !isGameOver && GameManager.Instance.isStarting)
        {
            //Debug.Log($"isJumping {isJumping}, isGameOver {isGameOver}, isStarting {GameManager.Instance.isStarting}");

            //Debug.Log("is in first if");
            if (!isElongated)
            {
                if (!isSimpleRolling && Input.GetKeyDown(KeyCode.Space))
                {
                    //Debug.Log("space keydown");
                    StartCharging();
                }

                if (!isSimpleRolling && Input.GetKey(KeyCode.Space) && isCharging)
                {
                    ContinueCharging();
                    ApplySquashEffect();
                }

                if (!isSimpleRolling && Input.GetKeyUp(KeyCode.Space))
                {
                    if (debugMode) Debug.Log("Space key released, attempting to jump");
                    Jump();
                    ResetSquashEffect();
                }
            }

            if (!isSimpleRolling)
            {
                HandleRollingInput();
                UpdateDirectionArrow();
            }
            if (isSimpleRolling && !isCharging && !isJumping)
            {
                UpdateRolling();
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                SaveGame();
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                LoadGame();
            }

            if (isElongated && !isJumping && !isSimpleRolling)
            {
                float scrollDelta = Input.mouseScrollDelta.y;
                if (scrollDelta != 0)
                {
                    // Convert scroll to integer change
                    int levelChange = scrollDelta > 0 ? 1 : -1;
                    ChangeElongateLevel(levelChange);
                }
            }
        }
        if (isJumping && isRolling)
        {
            // 在跳跃过程中应用翻滚效果

            //Debug.Log("Attempting to apply rolling effect");
            ApplyRollingEffect();
        }
        else if (isRolling)
        {
            // 在跳跃过程中应用翻滚效果
            //Debug.Log("Attempting to apply rolling effect");
            ApplyRollingEffect();
        }
        if (isInvincible)
        {
            playerRenderer.material = invincibleMaterial;
        }
        else
        {
            playerRenderer.material = originalMaterial;
        }

        // if (Input.GetMouseButtonDown(0) && Time.time - lastShootTime > shootCooldown && !isGameOver)
        // {
        //     Shoot();
        // }

        //if (goalPlatform != null && Vector3.Distance(transform.position, goalPlatform.position) < 0.5f)
        //{
        //    ReachGoal();
        //}
        // Increment the timer

        // Calculate remaining time

        if (isInvincible)
        {
            remainingTime = remainingTime - Time.deltaTime;
            timerText.text = $"Invincible Time Left: {remainingTime:F1}";
            if (remainingTime <= 0f)
            {
                timerText.text = "";
                isInvincible = false;
            }
        }
        if (isElongated)
        {
            remainingTime = remainingTime - Time.deltaTime;
            timerText.text = $"Elongate Level: {currentElongateLevel}/{maxElongateLevel}";

        }
        if (!isElongated && !isInvincible)
            timerText.text = "";


        // Update the UI Text to display remaining time

    }


    void FixedUpdate()
    {
        if (currentPlatform != null)
        {
            // 计算平台的速度
            Vector3 platformDelta = currentPlatform.position - lastPlatformPosition;
            platformVelocity = platformDelta / Time.fixedDeltaTime;

            // 使玩家跟随平台移动
            rb.MovePosition(rb.position + platformVelocity * Time.fixedDeltaTime);

            // 更新平台的上一个位置
            lastPlatformPosition = currentPlatform.position;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            currentPlatform = collision.transform;
            lastPlatformPosition = currentPlatform.position;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            currentPlatform = null;
        }
    }

    void UpdateDirectionArrow()
    {
        if (isShowingArrow && directionArrowImage != null)
        {
            // 计算箭头应该指向的角度
            float targetAngle = Mathf.Atan2(simpleRollDirection.z, simpleRollDirection.x) * Mathf.Rad2Deg;

            // 创建目标旋转（只在Y轴上旋转，保持水平）
            Quaternion targetRotation = Quaternion.Euler(0, -targetAngle, 0);

            // 平滑旋转箭头
            directionArrowImage.transform.rotation = Quaternion.Slerp(directionArrowImage.transform.rotation, targetRotation, arrowRotationSpeed * Time.deltaTime);
        }
    }

    void ShowDirectionArrow()
    {
        if (directionArrowImage != null)
        {
            directionArrowImage.gameObject.SetActive(true);
            isShowingArrow = true;
        }
    }

    void HideDirectionArrow()
    {
        if (directionArrowImage != null)
        {
            directionArrowImage.gameObject.SetActive(false);
            isShowingArrow = false;
        }
    }



    void ReachGoal()
    {
        Debug.Log("Goal reached! Moving to next level.");
        levelDisplayManager.SwitchToNextLevel();
        SetupLevel();
    }
    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position + transform.forward, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
        lastShootTime = Time.time;
        // SoundManager.Instance.PlayShootSound(); 
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.UpdateHp(health, 5);
    }

    // void UpdateCubeReferences()
    // {
    //     currentCube = platformManager.GetCurrentPlatform();
    //     nextCube = platformManager.GetNextPlatform();
    //     if (nextCube != null)
    //     {
    //         CalculateOptimalJumpForce();
    //     }
    // }

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
        //CalculateOptimalJumpForce();
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
            //Debug.Log($"Applying rotation. Total rotation: {totalRotation}, Roll axis: {rollAxis}"); // 调试日志
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
        //if (isCharging && nextCube != null)
        if (isCharging)
        {
            HideDirectionArrow();
            turnOnHorizontalPhysics();
            Vector3 jumpDirection = CalculateJumpDirection();
            //Vector3 jumpDirection = new Vector3(simpleRollHorizontal, 0, simpleRollVertical).normalized;
            // 考虑平台速度
            Vector3 initialVelocity = platformVelocity;
            rb.velocity = initialVelocity;
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

            SoundManager.Instance.PlayJumpSound();  
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
        //Vector3 targetDirection = nextCube.transform.position - transform.position;
        Vector3 targetDirection = new Vector3(simpleRollHorizontal, 0, simpleRollVertical).normalized;
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
        if (nowHasTargetCube && targetCube.transform.position == hitPlatform.transform.position)
            nowHasTargetCube = false;



        if (!isSimpleRolling && !nowHasTargetCube)
        {
            if (hitPlatform.CompareTag("Terrain") && !isFailed)
            {
                FailJump();
            }
        }

        if (hitPlatform.CompareTag("Coin"))
        {
            score += 1;
            Destroy(hitPlatform);
        }
        if (hitPlatform.CompareTag("Powerup"))
        {
            Destroy(hitPlatform);
            remainingTime = 12f;
            StartCoroutine(InvincibilityTimer(12f));
        }

        if (!isSimpleRolling && !nowHasTargetCube)
        {
            if (hitPlatform == transform.position.y > hitPlatform.transform.position.y + 0.6)
            {
                ShowDirectionArrow();
                SucceedJump(hitPlatform);
            }
            isRolling = false;

            // transform.rotation = Quaternion.identity;
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }


        // 处理移动平台
        if (hitPlatform.CompareTag("Moving"))
        {
            currentPlatform = hitPlatform.transform;
            lastPlatformPosition = currentPlatform.position;
        }
        else
        {
            currentPlatform = null;
            lastPlatformPosition = Vector3.zero;
            platformVelocity = Vector3.zero;
            //lastPlatformPosition = null;
        }

        if (hitPlatform.CompareTag("Terrain") && !isFailed)
        {
            FailJump();
        }

        // longer
        if (hitPlatform.CompareTag("Elongate"))
        {
            StartElongateEffect();
        }

        if (hitPlatform.CompareTag("Checkpoint") || hitPlatform.CompareTag("Start"))
        {
            SaveGame();
        }

        if (hitPlatform.CompareTag("Platform") || hitPlatform.CompareTag("Start"))
        {
            recoveredPlatform = hitPlatform;
        }




        // reach goal
        // if (hitPlatform.CompareTag("Goal"))
        // {
        //     Debug.Log("Congratulations! You've reached the goal!");
        //     StartCoroutine(CompleteLevel());
        //     return;
        // }
    }

    private IEnumerator InvincibilityTimer(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }

    private void SucceedJump(GameObject hitPlatform)
    {
        isJumping = false;
        //platformManager.MoveToNextPlatform();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = hitPlatform.transform.position + Vector3.up * (0.5f + transform.localScale.y);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        CheckGoalReached();
    }


    // private bool IsPlayerOnCurrentPlatform()
    // {
    //     return IsPlayerOnCube(platformManager.GetCurrentPlatform());
    // }

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

    public void FailJump()
    {
        SoundManager.Instance.PlayShatterSound();
        if (!isInvincible)
            health -= 1;
        healthBar.UpdateHp(health, 5);
        if (health <= 0)
        {
            isJumping = false;
            isRolling = false;
            isFailed = true;
            isGameOver = true;
            //Invoke("ShatterPlayer", shatterDelay);
            GameObject shatteredPlayer1 = Instantiate(shatteredPlayerPrefab, transform.position, Quaternion.identity);
            //Destroy(gameObject);
            SoundManager.Instance.PlayShatterSound();
            StartCoroutine(ShowGameOverAfterDelay());
        }
        else
        {
            isFailed = true;
            GameObject shatteredPlayer1 = Instantiate(shatteredPlayerPrefab, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            Invoke("recoverPlayer", 0.5f);
            //recoverPlayer();


        }


    }

    private IEnumerator ShowGameOverAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver();
            // 如果使用的是SetActive来隐藏/显示
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
        else
        {
            Debug.LogError("GameOverUI is not set!");
        }
    }


    private void recoverPlayer()
    {
        gameObject.SetActive(true);
        SucceedJump(recoveredPlatform);
        isFailed = false;
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

    public int getCurrentHealth()
    {
        return health;
    }

    public void SetHealth(int newHealth)
    {
        health = newHealth;
    }



    public void ResetGameState()
    {
        if (debugMode) Debug.Log("Resetting player game state...");
        // 如果使用的是SetActive来隐藏/显示
        gameObject.SetActive(true);
        isGameOver = false;
        isFailed = false;

        // if (platformManager != null && platformManager.GetCurrentPlatform() != null)
        // {
        //     transform.position = platformManager.GetCurrentPlatform().transform.position + Vector3.up;
        //     if (debugMode) Debug.Log($"Reset player position to: {transform.position}");
        // }
        // else
        // {
        //     transform.position = startPosition;
        //     if (debugMode) Debug.Log($"Reset player position to start position: {startPosition}");
        // }
        // transform.rotation = startRotation;

        // isGameOver = false;
        // isJumping = false;
        // isCharging = false;
        // currentJumpForce = 0f;

        // StopAllCoroutines();

        // if (rb != null)
        // {
        //     rb.velocity = Vector3.zero;
        //     rb.angularVelocity = Vector3.zero;
        // }

        // if (jumpPowerUI != null)
        // {
        //     jumpPowerUI.SetPower(0f);
        // }

        // Renderer renderer = GetComponent<Renderer>();
        // if (renderer != null) renderer.enabled = true;

        // Collider collider = GetComponent<Collider>();
        // if (collider != null) collider.enabled = true;

        // UpdateCubeReferences();

        // if (debugMode)
        // {
        //     Debug.Log("Player game state reset completed");
        //     Debug.Log($"Current cube: {(currentCube != null ? currentCube.name : "null")}");
        //     Debug.Log($"Next cube: {(nextCube != null ? nextCube.name : "null")}");
        // }
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

    void HandleRollingInput()
    {
        bool hasInput = false;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            simpleRollHorizontal = -1f;
            simpleRollVertical = 0f;
            hasInput = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            simpleRollHorizontal = 1f;
            simpleRollVertical = 0f;
            hasInput = true;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            simpleRollHorizontal = 0f;
            simpleRollVertical = -1f;
            hasInput = true;
        }
        else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            simpleRollHorizontal = 0f;
            simpleRollVertical = 1f;
            hasInput = true;
        }
        // Normalize the input to get a direction vector
        simpleRollDirection = new Vector3(simpleRollHorizontal, 0, simpleRollVertical).normalized;

        if (hasInput)
        {
            ShowDirectionArrow();  // 这里调用显示箭头
        }
        // else
        // {
        //     HideDirectionArrow();  // 这里调用隐藏箭头
        // }

        //if (Input.GetKeyDown(KeyCode.Return) && simpleRollDirection.magnitude > 0.1f)
        //{
        //    StartRolling();
        //    HideDirectionArrow();  // 开始滚动时隐藏箭头
        //}

        // Start rolling when Enter is pressed and we have a valid direction
        if (simpleRollDirection.magnitude > 0.1f && Input.GetKey(KeyCode.Return) && !isCharging && !isJumping)
        {
            StartRolling();
            HideDirectionArrow();
        }
    }

    void StartRolling()
    {
        if (!isSimpleRolling)
        {
            isSimpleRolling = true;
            simpleRollTimer = 0f;

            //GetComponent<Collider>().enabled = false;
        }
    }

    public void turnOffHorizontalPhysics()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }
    public void turnOnHorizontalPhysics()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
    }


    void UpdateRolling()
    {
        simpleRollTimer += Time.deltaTime;

        if (simpleRollTimer < simpleRollDuration)
        {
            // Calculate rotation for this frame
            float rotationThisFrame = simpleRollSpeed * Time.deltaTime * 0.1f;

            // Rotate around the axis perpendicular to both up vector and roll direction
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, simpleRollDirection).normalized;
            transform.Rotate(rotationAxis, rotationThisFrame, Space.World);

            // Move the player in the roll direction
            float moveDistance = (simpleRollSpeed * Mathf.PI * transform.localScale.y / simpleRollSpeed) * Time.deltaTime * 1.05f;
            transform.position += simpleRollDirection * moveDistance;
        }
        else
        {
            // End rolling
            isSimpleRolling = false;
            rb.isKinematic = false;
            //GetComponent<Collider>().enabled = true;
            ShowDirectionArrow();

            // Ensure the player is upright at the end of the roll
            //Vector3 uprightRotation = new Vector3(0, transform.eulerAngles.y, 0);
            //transform.rotation = Quaternion.Euler(uprightRotation);
            CheckGoalReached();
        }
    }

    public void turnOffPhysics()
    {
        // Disable rigidbody physics during roll
        rb.isKinematic = true;
    }
    public void turnOnPhysics()
    {
        // Disable rigidbody physics during roll
        rb.isKinematic = false;
    }
    void CheckGoalReached()
    {
        if (isLevelCompleting) return;
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Goal"))
            {
                Debug.Log("Congratulations! You've reached the goal!");
                StartCoroutine(CompleteCurrentLevel());
                break;
            }
        }
    }

    private IEnumerator CompleteCurrentLevel()
    {
        if (isLevelCompleting) yield break;

        isLevelCompleting = true;
        Debug.Log("Congratulations! You've reached the goal!");

        // 等待x秒
        yield return new WaitForSeconds(0.5f);

        levelDisplayManager.SwitchToNextLevel();
        SetupLevel();
        isLevelCompleting = false;
    }

    private void SaveGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();
            Debug.Log("Game saved.");
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
    }

    private void LoadGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGame();

            Debug.Log("Game loaded.");
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
    }

    public void setTargetCubeJumping(GameObject theGameObject)
    {
        if (!isJumping && !nowHasTargetCube)
        {
            targetCube = theGameObject;
            nowHasTargetCube = true;
            jumpToTarget();
        }

    }

    private void jumpToTarget()
    {
        if (nowHasTargetCube)
        {
            turnOnHorizontalPhysics();
            float exactJumpForce = CalculateTargetJumpForce();
            Vector3 jumpDirection = CalculateTargetJumpDirection();
            //Vector3 jumpDirection = new Vector3(simpleRollHorizontal, 0, simpleRollVertical).normalized;
            // 考虑平台速度
            Vector3 initialVelocity = platformVelocity;
            rb.velocity = initialVelocity;
            rb.AddForce(jumpDirection * exactJumpForce, ForceMode.Impulse);
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

        }
    }

    private float CalculateTargetJumpForce()
    {
        if (nowHasTargetCube)
        {
            Vector3 targetDirection = targetCube.transform.position - transform.position;
            float horizontalDistance = new Vector3(targetDirection.x, 0, targetDirection.z).magnitude;
            float verticalDistance = targetDirection.y;
            float gravity = Physics.gravity.magnitude;
            float radianAngle = jumpAngle * Mathf.Deg2Rad;

            float v0Squared = (gravity * horizontalDistance * horizontalDistance) /
                              (2 * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle) *
                               (horizontalDistance * Mathf.Tan(radianAngle) - verticalDistance));
            float v0 = Mathf.Sqrt(v0Squared);

            float exactJumpForce = v0 * rb.mass * forceAdjustment + 0.5f;

            return exactJumpForce;

        }

        return 0.0f;
    }

    private Vector3 CalculateTargetJumpDirection()
    {
        if (nowHasTargetCube)
        {
            //Vector3 targetDirection = nextCube.transform.position - transform.position;
            Vector3 targetDirection = targetCube.transform.position - transform.position;
            float horizontalDistance = new Vector3(targetDirection.x, 0, targetDirection.z).magnitude;
            float radianAngle = jumpAngle * Mathf.Deg2Rad;

            float cosAngle = Mathf.Cos(radianAngle);
            float tanAngle = Mathf.Tan(radianAngle);

            Vector3 horizontalDir = new Vector3(targetDirection.x, 0, targetDirection.z).normalized;
            Vector3 jumpDirection = horizontalDir * cosAngle + Vector3.up * tanAngle;

            return jumpDirection.normalized;
        }
        return new Vector3();
    }
    private void ChangeElongateLevel(int change)
    {
        int newLevel = Mathf.Clamp(currentElongateLevel + change, 1, maxElongateLevel);
        if (newLevel != currentElongateLevel)
        {
            currentElongateLevel = newLevel;
            UpdatePlayerLength();
            Debug.Log($"Elongate Level: {currentElongateLevel}/{maxElongateLevel}"); // Debug information
        }
    }

    private void StartElongateEffect()
    {
        if (!isElongated)
        {
            if (elongateCoroutine != null)
            {
                StopCoroutine(elongateCoroutine);
            }
            currentElongateLevel = maxElongateLevel; // Start at maximum level
            elongateCoroutine = StartCoroutine(ElongatePlayer());
        }
    }

    private IEnumerator ElongatePlayer()
    {
        isElongated = true;
        UpdatePlayerLength(); // Apply initial length
        Debug.Log($"Elongate effect started at level {currentElongateLevel}");

        yield return new WaitForSeconds(elongateDuration);

        // Reset all states
        currentElongateLevel = 1;
        transform.localScale = originalScale;
        isElongated = false;
        Debug.Log("Elongate effect ended");
    }

    private void UpdatePlayerLength()
    {
        // Calculate the actual elongation factor based on the current level
        float elongationFactor = 1f + (currentElongateLevel - 1) * baseElongateMultiplier;

        Vector3 newScale = new Vector3(
            originalScale.x,
            originalScale.y * elongationFactor,
            originalScale.z
        );
        transform.localScale = newScale;
    }


    private IEnumerator WaitCoroutine(float num)
    {
        yield return new WaitForSeconds(num);
        // Put any code here that you want to run after the delay
    }

    public void PlayerDead()
    {
        Invoke("ShatterPlayer", shatterDelay);
        //SoundManager.Instance.PlayShatterSound();
        StartCoroutine(ShowGameOverAfterDelay());
    }

    public bool isNowInvicible()
    {
        return isInvincible;
    }

    public void RecoverPlayerStatus()
    {
        health = 5;
        currentElongateLevel = 1;
        transform.localScale = originalScale;
        isElongated = false;
        isInvincible = false;
        score = 0;

        healthBar.UpdateHp(health, 5);
    }

}
