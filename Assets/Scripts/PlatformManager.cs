using UnityEngine;
using System.Collections.Generic;

public class PlatformManager : MonoBehaviour
{
    [Header("References")]
    public GameObject cubePrefab;
    public PlayerController playerController;

    [Header("Platform Generation Settings")]
    public float minDistance = 1f;
    public float maxDistance = 5f;
    public Vector3 initialPosition = new Vector3(10, 1, 10);
    [Range(0f, 1f)] public float xChangeProbability = 0.7f;
    [Range(0f, 1f)] public float positiveDirectionBias = 1.0f;
    private Vector3 platformScale = Vector3.one;

    [System.Serializable]
    public class ColorProbability
    {
        public Color color;
        public float probability;
    }

    public List<ColorProbability> colorProbabilities;

    [Header("Debug")]
    public bool debugMode = true;

    private List<GameObject> platforms = new List<GameObject>();
    private int currentPlatformIndex = 0;

    private void Start()
    {
        if (cubePrefab == null || playerController == null)
        {
            Debug.LogError("Cube Prefab or PlayerController is not assigned in PlatformManager!");
            return;
        }

        // 如果 colorProbabilities 为空或没有元素，添加默认值
        if (colorProbabilities == null || colorProbabilities.Count == 0)
        {
            colorProbabilities = new List<ColorProbability>
            {
                new ColorProbability { color = Color.red, probability = 0.2f },
                new ColorProbability { color = Color.blue, probability = 0.1f },
                new ColorProbability { color = Color.yellow, probability = 0.3f },
                new ColorProbability { color = Color.green, probability = 0.2f },
                new ColorProbability { color = Color.cyan, probability = 0.1f }
            };
        }

        CreatePlatform(initialPosition, "Initial", GetRandomColorExceptBlack());
        GenerateNextPlatform();
        playerController.OnJumpSuccess += GenerateNextPlatform;

        if (debugMode)
        {
            Debug.Log("PlatformManager initialized with the following color probabilities:");
            foreach (var cp in colorProbabilities)
            {
                Debug.Log($"Color: {cp.color}, Probability: {cp.probability}");
            }
        }
    }

    private void CreatePlatform(Vector3 position, string directionTag, Color color)
    {
        GameObject platform = Instantiate(cubePrefab, position, Quaternion.identity);
        platform.name = "Platform_" + platforms.Count;
        platform.transform.localScale = platformScale;
        Renderer renderer = platform.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
            if (debugMode) Debug.Log($"Created platform at {position} with direction {directionTag} and color {color}");
        }
        else
        {
            Debug.LogError("Platform doesn't have a Renderer component!");
        }

        PlatformInfo platformInfo = platform.AddComponent<PlatformInfo>();
        platformInfo.directionTag = directionTag;
        platformInfo.color = color;

        platforms.Add(platform);
    }

    public void SetPlatformScale(Vector3 scale)
    {
        platformScale = scale;
        UpdatePlatformScales();
    }

    private void UpdatePlatformScales()
    {
        foreach (GameObject platform in platforms)
        {
            platform.transform.localScale = platformScale;
        }
    }

    private void UpdatePlatformTags()
    {
        for (int i = 0; i < platforms.Count; i++)
        {
            if (i == currentPlatformIndex)
            {
                platforms[i].tag = "CurrentCube";
            }
            else if (i == currentPlatformIndex + 1)
            {
                platforms[i].tag = "nextCube";
            }
            else
            {
                platforms[i].tag = "Untagged";
            }
        }
    }

    public void MoveToNextPlatform()
    {
        if (currentPlatformIndex < platforms.Count - 1)
        {
            currentPlatformIndex++;
            if (currentPlatformIndex == platforms.Count - 1)
            {
                GenerateNextPlatform();
            }
        }
    }

    public GameObject GetCurrentPlatform()
    {
        return platforms[currentPlatformIndex];
    }

    public GameObject GetPreviousPlatform()
    {
        if (currentPlatformIndex > 0)
        {
            return platforms[currentPlatformIndex - 1];
        }
        return null;
    }

    public GameObject GetNextPlatform()
    {
        if (currentPlatformIndex < platforms.Count - 1)
        {
            return platforms[currentPlatformIndex + 1];
        }
        return null;
    }

    private void GenerateNextPlatform()
    {
        if (platforms.Count == 0)
        {
            Debug.LogError("No initial platform to generate from!");
            return;
        }

        Vector3 lastPosition = platforms[platforms.Count - 1].transform.position;
        Vector3 newPosition = GenerateNextPosition(lastPosition);
        string directionTag = GetDirectionTag(lastPosition, newPosition);
        Color color = GetRandomColor();
        CreatePlatform(newPosition, directionTag, color);
    }

    private Vector3 GenerateNextPosition(Vector3 currentPosition)
    {
        float distance = Random.Range(minDistance, maxDistance);
        Vector3 newPosition = currentPosition;

        if (Random.value < xChangeProbability)
        {
            newPosition.x += (Random.value < positiveDirectionBias) ? distance : -distance;
        }
        else
        {
            newPosition.z += (Random.value < positiveDirectionBias) ? distance : -distance;
        }
        return newPosition;
    }

    private string GetDirectionTag(Vector3 fromPosition, Vector3 toPosition)
    {
        Vector3 direction = toPosition - fromPosition;
        if (direction.x > 0) return "Right";
        if (direction.x < 0) return "Left";
        if (direction.z > 0) return "Forward";
        if (direction.z < 0) return "Back";
        return "Unknown";
    }

    private Color GetRandomColor()
    {
        float totalProbability = 0f;
        foreach (var cp in colorProbabilities)
        {
            totalProbability += cp.probability;
        }

        float randomValue = Random.Range(0f, totalProbability);
        float currentSum = 0f;

        if (debugMode) Debug.Log($"GetRandomColor: Total probability: {totalProbability}, Random value: {randomValue}");

        foreach (var cp in colorProbabilities)
        {
            currentSum += cp.probability;
            if (debugMode) Debug.Log($"Checking color: {cp.color}, Probability: {cp.probability}, Current sum: {currentSum}");
            if (randomValue <= currentSum)
            {
                if (debugMode) Debug.Log($"Selected color: {cp.color}");
                return cp.color;
            }
        }

        Debug.LogWarning("Failed to select a color, returning white as default.");
        return Color.white;
    }

    private Color GetRandomColorExceptBlack()
    {
        Color color;
        do
        {
            color = GetRandomColor();
        } while (color == Color.black);
        return color;
    }

    public void ResetPlatforms()
    {
        if (debugMode) Debug.Log("Resetting platforms...");

        foreach (GameObject platform in platforms)
        {
            Destroy(platform);
        }
        platforms.Clear();

        currentPlatformIndex = 0;

        CreatePlatform(initialPosition, "Initial", GetRandomColorExceptBlack());
        GenerateNextPlatform();

        UpdatePlatformTags();

        if (debugMode)
        {
            Debug.Log($"Platforms reset. Current platform count: {platforms.Count}");
            Debug.Log($"Initial platform position: {platforms[0].transform.position}");
            if (platforms.Count > 1)
            {
                Debug.Log($"Next platform position: {platforms[1].transform.position}");
            }
        }
    }
}

public class PlatformInfo : MonoBehaviour
{
    public string directionTag;
    public Color color;
}