using UnityEngine;
using System.Collections.Generic;

public class LevelDisplayManager : MonoBehaviour
{
    public List<LevelData> levels;
    public GameObject platformPrefab;
    public float groundSize = 100f; // Plane 的大小

    private int currentLevelIndex = 0;
    private GameObject currentLevelObject;

    void Start()
    {   
        AddGeneratedLevels();
        DisplayCurrentLevel();
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
    
        levels.Add(MapGenerator.GenerateLevel("Easy"));
        levels.Add(MapGenerator.GenerateLevel("Medium"));
        levels.Add(MapGenerator.GenerateLevel("Fire"));
        levels.Add(MapGenerator.GenerateLevel("Hard"));
        levels.Add(MapGenerator.GenerateLevel("MixedPath"));
        levels.Add(MapGenerator.GenerateLevel("IslandPath"));
        levels.Add(MapGenerator.GenerateLevel("SpiralPath"));
        levels.Add(MapGenerator.GenerateLevel("ScatteredIslands"));

        levels.Add(MazeGenerator.GenerateMazeLevel("Maze Level 1", 15, 15));
        levels.Add(MazeGenerator.GenerateMazeLevel("Maze Level 2", 20, 20));
   
    }
    public LevelData GetCurrentLevelData()
    {
        return levels[currentLevelIndex];
    }

    public void SwitchToNextLevel()
    {
        currentLevelIndex = (currentLevelIndex + 1) % levels.Count;
        DisplayCurrentLevel();
    }



    void DisplayCurrentLevel()
    {
     if (currentLevelObject != null)
        {
            Destroy(currentLevelObject);
        }

        currentLevelObject = new GameObject($"Level_{currentLevelIndex}");
        CreateGround(currentLevelObject.transform);
        DisplayLevel(levels[currentLevelIndex], currentLevelObject.transform);

        // 通知 PlayerController 重置玩家位置
        FindObjectOfType<PlayerController>().SetupLevel();
    }

    void DisplayLevel(LevelData levelData, Transform parent)
    {
        foreach (var platformData in levelData.platforms)
        {
            CreatePlatform(platformData, parent);
        }
    }

    void CreatePlatform(LevelData.PlatformData platformData, Transform parent)
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
        else
        {
            platform.tag = "Platform";
        }
        
        Rigidbody rb = platform.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = platform.AddComponent<Rigidbody>();
        }

        // Freeze position and rotation on all axes
        rb.constraints = RigidbodyConstraints.FreezePositionX |
                         RigidbodyConstraints.FreezePositionY |
                         RigidbodyConstraints.FreezePositionZ |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;
    }

    void CreateGround(Transform parent)
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.SetParent(parent, false);
        plane.transform.localPosition = Vector3.zero;
        plane.transform.localScale = new Vector3(groundSize / 10f, 1f, groundSize / 10f);
        plane.tag = "Terrain";
        Renderer renderer = plane.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(0.5f, 0.5f, 0.5f); // 中灰色
        }
    }

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
}
