using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public static LevelData GenerateLevel(string layoutName, float timeLimit = 60f, float platformSpacing = 0.01f)
    {
        int[,] selectedLayout;
        Dictionary<(int, int), (int, int)> selectedConnections = new Dictionary<(int, int), (int, int)>();
        Dictionary<(int, int), (float, float, float, float)> selectedMoving = new Dictionary<(int, int), (float, float, float, float)>();
        Dictionary<(int, int), int> enemyTypes = new Dictionary<(int, int), int>();
        Dictionary<(int, int), int> enemyRewardTypes = new Dictionary<(int, int), int>();
        Dictionary<(int, int), (int, int)[]> selectedTrigger = new Dictionary<(int, int), (int, int)[]>();
        Dictionary<(int, int), int> selectedWindowsShowing = new Dictionary<(int, int), int>();
        switch (layoutName)
        {
            case "Easy":
                selectedLayout = MapLayouts.EasyMazeLayout;
                selectedMoving = MapLayouts.EasyMazeMoving;
                selectedWindowsShowing = MapLayouts.EasyMazeWindowsShowing;

                //selectedConnections = MapLayouts.EasyMazeConnections;
                break;
            case "EasyTesting":
                selectedLayout = MapLayouts.EasyMazeTestingLayout;
                selectedConnections = MapLayouts.EasyMazeTestingConnections;
                enemyTypes = MapLayouts.EasyMazeTestingEnemyTypes;
                enemyRewardTypes = MapLayouts.EasyMazeTestingEnemyRewardTypes;
                break;
            case "Medium":
                selectedLayout = MapLayouts.ObstacleLayout;
                break;
            case "Hard":
                selectedLayout = MapLayouts.HardRollingLayout;
                break;
            case "Fire":
                selectedLayout = MapLayouts.FireLayout;
                selectedConnections = MapLayouts.FireLayoutConnections;
                break;

            case "FireTesting":
                selectedLayout = MapLayouts.FireTestingLayout;
                selectedConnections = MapLayouts.FireTestingLayoutConnections;
                selectedTrigger = MapLayouts.FireTestingLayoutTrigger;
                break;
            case "FireTesting2":
                selectedLayout = MapLayouts.FireTesting2Layout;
                selectedTrigger = MapLayouts.FireTesting2LayoutTrigger;
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
                selectedMoving = MapLayouts.AdvancedLayoutMoving;
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
                if (selectedLayout[z, x] > -1)
                {
                    LevelData.PlatformData platform = new LevelData.PlatformData
                    {
                        position = new Vector3(x * (1 + platformSpacing), 5.0f, z * (1 + platformSpacing)),
                        scale = Vector3.one,

                        directionTag = ""
                    };

                    if (selectedWindowsShowing.ContainsKey((z, x)))
                    {
                        platform.isPopUp = true;
                        platform.popUpIndex = selectedWindowsShowing[(z, x)];
                    }

                    if (selectedLayout[z, x] == 0)
                    {
                        platform.type = LevelData.PlatformType.Empty;
                        platform.color = Color.clear;
                    }

                    else if (selectedLayout[z, x] == 2)
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
                        platform.type = LevelData.PlatformType.Invincible;
                        platform.color = Color.yellow;
                    }
                    else if (selectedLayout[z, x] == 8)
                    {
                        platform.type = LevelData.PlatformType.Coin;
                        platform.color = Color.yellow;
                    }
                    else if (selectedLayout[z, x] == 9)
                    {
                        var moveData = selectedMoving[(z, x)];  // Get the tuple from dictionary
                        platform.type = LevelData.PlatformType.Moving;
                        platform.color = Color.green;
                        platform.isMoving = true;
                        platform.moveStart = platform.position;
                        platform.moveEnd = platform.position + new Vector3(moveData.Item1, moveData.Item2, moveData.Item3);
                        platform.moveDuration = moveData.Item4;
                    }
                    else if (selectedLayout[z, x] == 11)
                    {
                        platform.type = LevelData.PlatformType.Elongate;
                        platform.color = Color.magenta; // Choose a distinct color for the new platform type
                    }
                    else if (selectedLayout[z, x] == 12)
                    {
                        platform.type = LevelData.PlatformType.EnemySrc;
                        platform.color = Color.red;
                    }
                    else if (selectedLayout[z, x] == 13)
                    {
                        platform.type = LevelData.PlatformType.SpringsTrigger;
                        platform.color = Color.green;
                    }
                    else if (selectedLayout[z, x] == 14)
                    {
                        platform.type = LevelData.PlatformType.HiddenTrigger;
                        platform.color = Color.white;
                    }
                    else if (selectedLayout[z, x] == 15)
                    {
                        platform.type = LevelData.PlatformType.Hidden;
                        platform.color = Color.clear;
                    }
                    else if (selectedLayout[z, x] == 22)
                    {
                        platform.type = LevelData.PlatformType.Checkpoint;
                        platform.color = Color.grey;
                    }
                    else
                    {
                        platform.type = LevelData.PlatformType.Normal;
                        platform.color = Color.cyan;
                    }

                    platform.zInArray = z;
                    platform.xInArray = x;
                    levelData.platforms.Add(platform);
                }
            }
        }
        levelData.platformsConnections = new Dictionary<(int, int), (int, int)>();
        foreach (var connection in selectedConnections)
        {
            levelData.platformsConnections[connection.Key] = connection.Value;
        }
        levelData.Trigger = new Dictionary<(int, int), (int, int)[]>();
        foreach (var connection in selectedTrigger)
        {
            levelData.Trigger[connection.Key] = connection.Value;
        }

        levelData.platformsEnemyTypes = new Dictionary<(int, int), int>();
        foreach (var connection in enemyTypes)
        {
            levelData.platformsEnemyTypes[connection.Key] = connection.Value;
        }

        levelData.platformsEnemyRewardTypes = new Dictionary<(int, int), int>();
        foreach (var connection in enemyRewardTypes)
        {
            levelData.platformsEnemyTypes[connection.Key] = connection.Value;
        }
        // levelData.windowsShowing = new Dictionary<(int, int), int>();
        // foreach (var connection in selectedWindowsShowing)
        // {
        //     levelData.windowsShowing[connection.Key] = connection.Value;
        // }

        return levelData;
    }

}
