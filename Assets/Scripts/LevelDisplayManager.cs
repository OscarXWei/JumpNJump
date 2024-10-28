using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class LevelDisplayManager : MonoBehaviour
{
    public List<LevelData> levels;
    public GameObject platformPrefab;
    //public float groundSize = 100f; // Plane 的大小

    private int currentLevelIndex = 0;
    private GameObject currentLevelObject;
    private CameraController cameraController;

    // Add references for background objects
    public GameObject backGround1;
    public GameObject backGround2;
    public GameObject backGround3;
    public GameObject backGround4;
    public GameObject coin;
    public GameObject invincible;
    public GameObject enemyPrefab;
    public GameObject hidden;

    public Material grass;
    public Material sand;
    public Material snow;
    public Material alien;

    void Start()
    {
        AddGeneratedLevels();
        DisplayCurrentLevel();
        cameraController = FindObjectOfType<CameraController>();
        // if (PlayerPrefs.HasKey("LevelToLoad"))
        // {
        //     int levelIndex = PlayerPrefs.GetInt("LevelToLoad");
        //     currentLevelIndex = levelIndex;
        //     DisplayCurrentLevel();

        //     PlayerPrefs.DeleteKey("LevelToLoad");
        // }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            SwitchToPreviousLevel();
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            SwitchToNextLevel();
        }

        if (currentLevelIndex == 0)
        {
            backGround1.SetActive(true);
            backGround2.SetActive(false);
            backGround3.SetActive(false);
            backGround4.SetActive(false);
        }
        else if (currentLevelIndex == 1)
        {
            backGround1.SetActive(false);
            backGround2.SetActive(true);
            backGround3.SetActive(false);
            backGround4.SetActive(false);
        }
        else if (currentLevelIndex == 2)
        {
            backGround1.SetActive(false);
            backGround2.SetActive(false);
            backGround3.SetActive(true);
            backGround4.SetActive(false);
        }
        else if (currentLevelIndex == 3)
        {
            backGround1.SetActive(false);
            backGround2.SetActive(false);
            backGround3.SetActive(false);
            backGround4.SetActive(true);
        }
    }

    void AddGeneratedLevels()
    {

        levels = new List<LevelData>();

        //tutorials
        levels.Add(MapGenerator.GenerateLevel("MixedPath")); //rolling
        levels.Add(MapGenerator.GenerateLevel("IslandPath")); //jumping
        levels.Add(MapGenerator.GenerateLevel("Easy")); //moving
        levels.Add(MapGenerator.GenerateLevel("Medium")); //elongating
        levels.Add(MapGenerator.GenerateLevel("Fire")); //spring
        levels.Add(MapGenerator.GenerateLevel("EnemyTut")); //enemy
        levels.Add(MapGenerator.GenerateLevel("FireTesting")); //spring trigger 
        levels.Add(MapGenerator.GenerateLevel("FireTesting2")); //hidden trigger

        //levels
        levels.Add(MapGenerator.GenerateLevel("AdvancedLayout"));
        levels.Add(MapGenerator.GenerateLevel("Fire"));

        //levels.Add(MapGenerator.GenerateLevel("EasyTesting"));




        // levels.Add(MapGenerator.GenerateLevel("Fire"));
        // levels.Add(MapGenerator.GenerateLevel("Hard"));
        // levels.Add(MapGenerator.GenerateLevel("SpiralPath"));
        //levels.Add(MapGenerator.GenerateLevel("ScatteredIslands"));



        // levels.Add(MazeGenerator.GenerateMazeLevel("Maze Level 1", 15, 15));
        // levels.Add(MazeGenerator.GenerateMazeLevel("Maze Level 2", 20, 20));

    }
    public LevelData GetCurrentLevelData()
    {
        if (currentLevelIndex == 0)
        {
            backGround1.SetActive(true);
            backGround2.SetActive(false);
            backGround3.SetActive(false);
            backGround4.SetActive(false);
        }
        return levels[currentLevelIndex];
    }

    public void SwitchToNextLevel()
    {
        // 保存当前关卡索引
        int previousIndex = currentLevelIndex;

        // 计算下一关索引
        currentLevelIndex = (currentLevelIndex + 1) % levels.Count;

        // 显示新关卡
        DisplayCurrentLevel();

        FindObjectOfType<PlayerController>().RecoverPlayerStatus();

        Debug.Log($"Switching from level {previousIndex} to level {currentLevelIndex}");
    }

    private void UpdateBackground()
    {
        // 关闭所有背景
        backGround1.SetActive(false);
        backGround2.SetActive(false);
        backGround3.SetActive(false);
        backGround4.SetActive(false);

        // 根据当前关卡索引激活对应背景
        switch (currentLevelIndex)
        {
            case 0:
                backGround1.SetActive(true);
                break;
            case 1:
                backGround2.SetActive(true);
                break;
            case 2:
                backGround3.SetActive(true);
                break;
            case 3:
                backGround4.SetActive(true);
                break;
        }
    }


    void DisplayCurrentLevel()
    {
        if (currentLevelObject != null)
        {
            Destroy(currentLevelObject);
        }

        currentLevelObject = new GameObject($"Level_{currentLevelIndex}");
        DisplayLevel(levels[currentLevelIndex], currentLevelObject.transform);

        // 通知 PlayerController 重置玩家位置
        FindObjectOfType<PlayerController>().SetupLevel();

        // 触发相机运镜效果
        if (cameraController != null)
        {
            cameraController.PlayIntroAnimation();
        }
    }

    void DisplayLevel(LevelData levelData, Transform parent)
    {
        foreach (var platformData in levelData.platforms)
        {
            GameObject thisOne = CreatePlatform(platformData, parent);
            levelData.platformPositions[thisOne] = (platformData.zInArray, platformData.xInArray);
            levelData.positionPlatforms[(platformData.zInArray, platformData.xInArray)] = thisOne;

        }
    }

    public GameObject CreatePlatform(LevelData.PlatformData platformData, Transform parent)
    {
        GameObject platform = Instantiate(platformPrefab, platformData.position, Quaternion.identity, parent);
        platform.transform.localScale = platformData.scale;
        Renderer renderer = platform.GetComponent<Renderer>();
        if (platformData.isPopUp)
        {
            platform.SendMessage("SetPopIndex", platformData.popUpIndex);
        }
        if (renderer != null)
        {
            // Set up material for transparency
            Material material = renderer.material;
            material.SetFloat("_Mode", 3); // Set to transparent mode
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;

            renderer.material.color = platformData.color;
        }
        if (platformData.type == LevelData.PlatformType.Goal)
        {
            platform.tag = "Goal";
        }
        else if (platformData.type == LevelData.PlatformType.Empty)
        {
            platform.tag = "Empty";
            Destroy(platform);
        }
        else if (platformData.type == LevelData.PlatformType.Explosive)
        {
            platform.tag = "Explosive";
        }
        else if (platformData.type == LevelData.PlatformType.Start)
        {
            platform.tag = "Start";
        }
        else if (platformData.type == LevelData.PlatformType.Checkpoint)
        {
            platform.tag = "Checkpoint";
        }
        else if (platformData.type == LevelData.PlatformType.Coin)
        {
            platform.tag = "coinPlatform";
            Vector3 coinPosition = platformData.position + new Vector3(0, 1f, 0);
            GameObject Coin = Instantiate(coin, coinPosition, Quaternion.identity, parent);
            Coin.tag = "Coin";
            Coin.AddComponent<ItemSpin>();
        }
        else if (platformData.type == LevelData.PlatformType.Invincible)
        {
            platform.tag = "powerupPlatform";
            Vector3 invinciblePosition = platformData.position + new Vector3(0, 1f, 0);
            GameObject Invincible = Instantiate(invincible, invinciblePosition, Quaternion.identity, parent);
            Invincible.tag = "Powerup";
            Invincible.AddComponent<ItemSpin>();
        }
        else if (platformData.type == LevelData.PlatformType.SpringStart)
        {
            platform.tag = "SpringStart";
        }
        else if (platformData.type == LevelData.PlatformType.EnemySrc)
        {
            platform.tag = "EnemySrc";
        }
        else if (platformData.type == LevelData.PlatformType.SpringsTrigger)
        {
            platform.tag = "SpringsTrigger";
        }
        else if (platformData.type == LevelData.PlatformType.Elongate)
        {
            platform.tag = "Elongate";
        }
        else if (platformData.type == LevelData.PlatformType.Normal)
        {
            if (currentLevelIndex == 0)
            {
                renderer.material = grass;
            }
            else if (currentLevelIndex == 1)
            {
                renderer.material = sand;
            }
            else if (currentLevelIndex == 2)
            {
                renderer.material = snow;
            }
            else if (currentLevelIndex == 3)
            {
                renderer.material = alien;
            }
            else
            {
                renderer.material = alien;
            }
            platform.tag = "Platform";
        }
        else if (platformData.type == LevelData.PlatformType.Moving)
        {
            platform.tag = "Moving";
        }
        else if (platformData.type == LevelData.PlatformType.Hidden)
        {
            Destroy(platform);
            GameObject hiddencube = Instantiate(hidden, platformData.position, Quaternion.identity, parent);
            hiddencube.tag = "Hidden";
        }
        else if (platformData.type == LevelData.PlatformType.HiddenTrigger)
        {
            platform.tag = "HiddenTrigger";
        }

        Rigidbody rb = platform.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = platform.AddComponent<Rigidbody>();
        }

        rb.isKinematic = true;

        if (platformData.isMoving)
        {
            StartMovingPlatform(platform.transform, platformData);
        }

        // Freeze position and rotation on all axes
        rb.constraints = RigidbodyConstraints.FreezePositionX |
                         RigidbodyConstraints.FreezePositionY |
                         RigidbodyConstraints.FreezePositionZ |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;
        return platform;
    }

    void StartMovingPlatform(Transform platformTransform, LevelData.PlatformData platformData)
    {
        StartCoroutine(MovePlatformCoroutine(platformTransform, platformData));
    }

    IEnumerator MovePlatformCoroutine(Transform platform, LevelData.PlatformData platformData)
    {
        Vector3 startPos = platformData.moveStart;
        Vector3 endPos = platformData.moveEnd;
        float duration = platformData.moveDuration;

        while (true)
        {
            yield return StartCoroutine(MoveFromTo(platform, startPos, endPos, duration));
            yield return StartCoroutine(MoveFromTo(platform, endPos, startPos, duration));
        }
    }

    IEnumerator MoveFromTo(Transform objectToMove, Vector3 a, Vector3 b, float duration)
    {
        float step = 0f;
        float rate = 1f / duration;
        while (step < 1.0f)
        {
            step += Time.deltaTime * rate;
            objectToMove.position = Vector3.Lerp(a, b, Mathf.SmoothStep(0f, 1f, step));
            yield return null;
        }
    }


    // void CreateGround(Transform parent)
    // {
    //     GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
    //     plane.transform.SetParent(parent, false);
    //     plane.transform.localPosition = Vector3.zero;
    //     plane.transform.localScale = new Vector3(groundSize / 10f, 1f, groundSize / 10f);
    //     plane.tag = "Terrain";
    //     Renderer renderer = plane.GetComponent<Renderer>();
    //     if (renderer != null)
    //     {
    //         renderer.material.color = new Color(0.5f, 0.5f, 0.5f); // 中灰色
    //     }
    // }

    // void SwitchToNextLevel()
    // {
    //     currentLevelIndex = (currentLevelIndex + 1) % levels.Count;
    //     DisplayCurrentLevel();
    // }

    void SwitchToPreviousLevel()
    {
        currentLevelIndex = (currentLevelIndex - 1 + levels.Count) % levels.Count;
        DisplayCurrentLevel();
    }

    public (bool success, GameObject val) findMatchedPlatform(GameObject theGameObject)
    {

        // Check if the given gameObject is in the platformPositions dictionary
        if (levels[currentLevelIndex].platformPositions.TryGetValue(theGameObject, out (int z, int x) position))
        {
            // Check if this position has a connection in the platformsConnections dictionary
            if (levels[currentLevelIndex].platformsConnections.TryGetValue(position, out (int targetZ, int targetX) targetPosition))
            {
                // Check if there's a platform at the target position
                if (levels[currentLevelIndex].positionPlatforms.TryGetValue(targetPosition, out GameObject targetPlatform))
                {
                    return (true, targetPlatform);
                }
            }
        }

        // If no match was found, return false and the original gameObject
        return (false, theGameObject);
    }
    public (bool success, int val) findPlatformEnemyType(GameObject theGameObject)
    {

        // Check if the given gameObject is in the platformPositions dictionary
        if (levels[currentLevelIndex].platformPositions.TryGetValue(theGameObject, out (int z, int x) position))
        {
            // Check if this position has a connection in the platformsConnections dictionary
            if (levels[currentLevelIndex].platformsEnemyTypes.TryGetValue(position, out int typeVal))
            {

                return (true, typeVal);

            }
        }

        // If no match was found, return false and the original gameObject
        return (false, 0);
    }

    public (bool success, int val) findPlatformEnemyRewardType(GameObject theGameObject)
    {

        // Check if the given gameObject is in the platformPositions dictionary
        if (levels[currentLevelIndex].platformPositions.TryGetValue(theGameObject, out (int z, int x) position))
        {
            // Check if this position has a connection in the platformsConnections dictionary
            if (levels[currentLevelIndex].platformsEnemyRewardTypes.TryGetValue(position, out int typeVal))
            {

                return (true, typeVal);

            }
        }

        // If no match was found, return false and the original gameObject
        return (false, 0);
    }

    public (bool success, GameObject[] vals) findTemp(GameObject theGameObject)
    {

        List<GameObject> objs = new List<GameObject>();
        // Check if the given gameObject is in the platformPositions dictionary
        if (levels[currentLevelIndex].platformPositions.TryGetValue(theGameObject, out (int z, int x) position))
        {
            // Check if this position has a connection in the platformsConnections dictionary
            if (levels[currentLevelIndex].Trigger.TryGetValue(position, out var arrayVal))
            {

                foreach (var pos in arrayVal)
                {
                    if (levels[currentLevelIndex].positionPlatforms.TryGetValue(pos, out GameObject targetPlatform))
                    {
                        objs.Add(targetPlatform);
                    }
                }

            }
        }

        // If no match was found, return false and the original gameObject
        return (true, objs.ToArray());
    }

    public void generateEnemy(int level, int rewardLevel, Vector3 cubePos)
    {

        Vector3 enemyPosition = cubePos + new Vector3(0, 1f, 0);
        GameObject enemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity, currentLevelObject.transform);
        enemy.SendMessage("setLevel", level);
        enemy.SendMessage("setRewardLevel", rewardLevel);


    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }

    public void SetCurrentLevelIndex(int index)
    {
        if (index >= 0 && index < levels.Count)
        {
            currentLevelIndex = index;
            DisplayCurrentLevel();
        }
        else
        {
            Debug.LogError("Invalid level index");
        }
    }

}
