using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public static LevelData GenerateLevel(string layoutName, float timeLimit = 60f, float platformSpacing = 0.01f)
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
            
            case "MixedPath":
                selectedLayout = MapLayouts.MixedPathLayout;
                break;
            case "IslandPath":
                selectedLayout = MapLayouts.IslandPathLayout;
                break;
            case "SpiralPath":
                selectedLayout = MapLayouts.SpiralPathLayout;
                break;
            case "ScatteredIslands":
                selectedLayout = MapLayouts.ScatteredIslandsLayout;
                break;
            case "AdvancedLayout":
                selectedLayout = MapLayouts.AdvancedLayout;
                break;
            default:
                Debug.LogError("Invalid layout name: " + layoutName);
                return null;
        }

        LevelData levelData = ScriptableObject.CreateInstance<LevelData>();
        levelData.levelName = layoutName; // 设置关卡名称
        levelData.timeLimit = timeLimit;
        levelData.checkpointIndex = -1;

        int height = selectedLayout.GetLength(0);
        int width = selectedLayout.GetLength(1);

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (selectedLayout[z, x] != 0)
                {
                    LevelData.PlatformData platform = new LevelData.PlatformData
                    {
                        position = new Vector3(x * (1 + platformSpacing), 0.5f, z * (1 + platformSpacing)),
                        scale = Vector3.one,
                        
                        directionTag = ""
                    };

                    if (selectedLayout[z, x] == 2)
                    {
                        platform.type = LevelData.PlatformType.Start;
                        platform.color = Color.blue;
                    }
                    else if (selectedLayout[z, x] == 3)
                    {
                        platform.type = LevelData.PlatformType.Goal;
                        platform.color = Color.red;
                    }
                    else if (selectedLayout[z, x] == 4)
                    {
                        platform.type = LevelData.PlatformType.Explosive;
                        platform.color = Color.black;
                    }
                    else if (selectedLayout[z, x] == 5)
                    {
                        platform.type = LevelData.PlatformType.SpringStart;
                        platform.color = Color.green;
                    }
                    else if (selectedLayout[z, x] == 6)
                    {
                        platform.type = LevelData.PlatformType.SprintEnd;
                        platform.color = Color.green;
                    }
                    else if (selectedLayout[z, x] == 7)
                    {
                        platform.type = LevelData.PlatformType.CoinTrigger;
                        platform.color = Color.yellow;
                    }
                    else if (selectedLayout[z, x] == 8)
                    {
                        platform.type = LevelData.PlatformType.Coin;
                        platform.color = Color.yellow;
                    }
                    else if (selectedLayout[z, x] == 9)
                    {
                        platform.type = LevelData.PlatformType.Moving;
                        platform.color = Color.green;
                        platform.isMoving = true;
                        platform.moveStart = platform.position;
                        platform.moveEnd = platform.position + new Vector3(0, 0, 2f); // 移动2个单位
                        platform.moveDuration = 2f; // 移动周期为2秒
                    }
                    else
                    {
                        platform.type = LevelData.PlatformType.Normal;
                        platform.color = Color.cyan;
                    }

                    levelData.platforms.Add(platform);
                }
            }
        }

        return levelData;
    }
}