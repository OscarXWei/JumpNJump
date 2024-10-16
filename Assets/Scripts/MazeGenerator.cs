using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator
{
    private static int[,] maze;
    private static int width;
    private static int height;
    private const float SPACING = 1f; 

    public static LevelData GenerateMazeLevel(string levelName, int mazeWidth, int mazeHeight)
    {
        width = mazeWidth;
        height = mazeHeight;
        maze = new int[width, height];

        // 初始化迷宫为全墙
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                maze[x, z] = 1;
            }
        }

        // 生成迷宫
        GenerateMaze(1, 1);

        // 创建 LevelData
        LevelData levelData = ScriptableObject.CreateInstance<LevelData>();
        levelData.levelName = levelName;
        levelData.playerStartPosition = new Vector3(SPACING, 1, SPACING);
        levelData.platforms = new List<LevelData.PlatformData>();

        // 将迷宫转换为平台
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (maze[x, z] == 1) // 墙
                {
                    LevelData.PlatformData wall = new LevelData.PlatformData
                    {
                        position = new Vector3(x * SPACING, 1, z * SPACING),
                        scale = new Vector3(1, 1, 1),
                        //color = Random.ColorHSV(), // 随机颜色
                        color = Color.cyan,
                        type = LevelData.PlatformType.Normal
                    };
                    levelData.platforms.Add(wall);
                }
            }
        }

        // 添加起点和终点
        AddStartAndGoal(levelData);

        return levelData;
    }

    private static void GenerateMaze(int x, int z)
    {
        maze[x, z] = 0; 
        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(0, 2), new Vector2Int(2, 0),
            new Vector2Int(0, -2), new Vector2Int(-2, 0)
        };
        Shuffle(directions);
        foreach (Vector2Int dir in directions)
        {
            int nx = x + dir.x;
            int nz = z + dir.y;
            if (nx > 0 && nx < width - 1 && nz > 0 && nz < height - 1 && maze[nx, nz] == 1)
            {
                maze[x + dir.x / 2, z + dir.y / 2] = 0;
                GenerateMaze(nx, nz);
            }
        }
    }

    private static void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private static void AddStartAndGoal(LevelData levelData)
    {
        // 起点
        LevelData.PlatformData start = new LevelData.PlatformData
        {
            position = new Vector3(SPACING, 1, SPACING),
            scale = Vector3.one,
            color = Color.green,
            type = LevelData.PlatformType.Start
        };
        levelData.platforms.Add(start);

        // 终点
        LevelData.PlatformData goal = new LevelData.PlatformData
        {
            position = new Vector3((width - 2) * SPACING, 1, (height - 2) * SPACING),
            scale = Vector3.one,
            color = Color.red,
            type = LevelData.PlatformType.Goal
        };
        levelData.platforms.Add(goal);
    }
}