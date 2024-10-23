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

    // Add references for background objects
    public GameObject backGround1;
    public GameObject backGround2;
    public GameObject backGround3;
    public GameObject backGround4;
    public GameObject coin;
    public GameObject invincible;

    void Start()
    {
        AddGeneratedLevels();
        DisplayCurrentLevel();
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
    }

    void AddGeneratedLevels()
    {

        levels = new List<LevelData>();

        levels.Add(MapGenerator.GenerateLevel("AdvancedLayout"));

        levels.Add(MapGenerator.GenerateLevel("Easy"));
        levels.Add(MapGenerator.GenerateLevel("Medium"));
        // levels.Add(MapGenerator.GenerateLevel("Fire"));
        // levels.Add(MapGenerator.GenerateLevel("Hard"));
        // levels.Add(MapGenerator.GenerateLevel("MixedPath"));
        // levels.Add(MapGenerator.GenerateLevel("IslandPath"));
        // levels.Add(MapGenerator.GenerateLevel("SpiralPath"));
        levels.Add(MapGenerator.GenerateLevel("ScatteredIslands"));



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
        currentLevelIndex = (currentLevelIndex + 1) % levels.Count;
        if (currentLevelIndex == 1)
        {
            backGround1.SetActive(false);
            backGround2.SetActive(true);
            backGround3.SetActive(false);
            backGround4.SetActive(false);
        }
        else if (currentLevelIndex == 2)
        {
            backGround2.SetActive(false);
            backGround1.SetActive(false);
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
        // else if (currentLevelIndex == 4)
        // {
        //     backGround1.SetActive(false);
        //     backGround2.SetActive(false);
        //     backGround3.SetActive(false);
        //     backGround4.SetActive(true);
        // }
        DisplayCurrentLevel();
    }



    void DisplayCurrentLevel()
    {
        if (currentLevelObject != null)
        {
            Destroy(currentLevelObject);
        }

        currentLevelObject = new GameObject($"Level_{currentLevelIndex}");
        //CreateGround(currentLevelObject.transform);
        DisplayLevel(levels[currentLevelIndex], currentLevelObject.transform);

        // 通知 PlayerController 重置玩家位置
        FindObjectOfType<PlayerController>().SetupLevel();
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
        if (renderer != null)
        {
            renderer.material.color = platformData.color;
        }
        if (platformData.type == LevelData.PlatformType.Goal)
        {
            platform.tag = "Goal";
        }
        if (platformData.type == LevelData.PlatformType.Explosive)
        {
            platform.tag = "Explosive";
        }
        if (platformData.type == LevelData.PlatformType.Start)
        {
            platform.tag = "Start";
        }
        if (platformData.type == LevelData.PlatformType.Coin)
        {
            platform.tag = "coinPlatform";
            Vector3 coinPosition = platformData.position + new Vector3(0, 1f, 0);
            GameObject Coin = Instantiate(coin, coinPosition, Quaternion.identity, parent);
            Coin.tag = "Coin";
        }
        if (platformData.type == LevelData.PlatformType.Invincible)
        {
            platform.tag = "powerupPlatform";
            Vector3 invinciblePosition = platformData.position + new Vector3(0, 1f, 0);
            GameObject Invincible = Instantiate(invincible, invinciblePosition, Quaternion.identity, parent);
            Invincible.tag = "Powerup";
        }
        if (platformData.type == LevelData.PlatformType.SpringStart)
        {
            platform.tag = "SpringStart";
        }
        if (platformData.type == LevelData.PlatformType.Elongate)
        {
            platform.tag = "Elongate";
        }
        if (platformData.type == LevelData.PlatformType.Normal)
        {
            platform.tag = "Platform";
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
