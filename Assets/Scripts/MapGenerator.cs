using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public static LevelData GenerateLevel(string layoutName, float timeLimit = 60f)
    {
        int[,] selectedLayout;
        switch (layoutName)
        {
            case "Easy":
                selectedLayout = MapLayouts.EasyMazeLayout;
                break;
            case "Medium":
                selectedLayout = MapLayouts.ObstacleLayout;
                break;
            case "Hard":
                selectedLayout = MapLayouts.HardRollingLayout;
                break;
            case "Fire":
                selectedLayout = MapLayouts.FireLayout;
                break;
            default:
                Debug.LogError("Invalid layout name: " + layoutName);
                return null;
        }

        LevelData levelData = ScriptableObject.CreateInstance<LevelData>();
        levelData.levelName = layoutName;  // 设置关卡名称
        levelData.timeLimit = timeLimit;
        levelData.checkpointIndex = -1;

        int height = selectedLayout.GetLength(0);
        int width = selectedLayout.GetLength(1);

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (selectedLayout[z, x] == 1)
                {
                    LevelData.PlatformData platform = new LevelData.PlatformData
                    {
                        position = new Vector3(x, 0, z),
                        scale = Vector3.one,
                        color = Color.blue,
                        type = LevelData.PlatformType.Normal,
                        directionTag = ""
                    };
                    levelData.platforms.Add(platform);
                }
            }
        }

        return levelData;
    }
}