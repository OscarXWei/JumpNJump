using System;
using System.Collections.Generic;
using UnityEngine;

public static class MapLayouts
{
    public static readonly int[,] HardRollingLayout = new int[,]
    {
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,1,1,1,1,0,0,0,0,0,0,0,0},
        {0,0,1,3,1,1,0,0,0,0,0,0,0,0},
        {0,0,1,1,1,0,0,0,0,0,0,0,0,0},
        {0,0,1,0,0,0,1,1,1,1,1,1,0,0},
        {0,0,8,0,0,0,1,1,0,0,1,1,0,0},
        {0,2,7,1,1,1,1,1,0,0,1,1,1,0},
        {0,0,0,0,0,0,1,1,0,0,0,0,1,0},
        {0,0,0,0,0,0,1,1,1,1,0,0,1,0},
        {0,0,0,0,0,0,1,1,1,1,1,1,1,0},
        {0,0,0,0,0,0,0,0,0,1,1,1,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0}
    };

    // public static readonly int[,] EasyMazeLayout = new int[,]
    // {
    //     {1,1,5,1,1,1,5,1,1,3},
    //     {1,0,0,0,0,0,0,0,0,1},
    //     {1,0,1,1,1,1,1,1,0,1},
    //     {1,0,1,0,0,0,0,1,0,1},
    //     {1,0,1,0,1,6,0,1,0,1},
    //     {1,0,1,0,1,1,0,1,0,1},
    //     {1,0,1,0,0,0,0,1,0,1},
    //     {1,0,6,1,1,1,1,1,0,1},
    //     {1,0,0,0,0,0,0,0,0,1},
    //     {2,7,8,1,1,1,1,1,1,1}
    // };

    public static readonly int[,] EasyMazeLayout = new int[,]
    {
            {2},
            {0},
            {0},
            {0},
            {0},
            {0},
            {0},
            {0},
            {9},
            {3}

    };

    public static readonly Dictionary<(int, int), (float, float, float, float)> EasyMazeMoving = new Dictionary<(int, int), (float, float, float, float)>
    {
        { (8, 0), (0f, 0f, -7f, 5f) }
    };

    // public static readonly Dictionary<(int, int), (int, int)> EasyMazeConnections = new Dictionary<(int, int), (int, int)>
    // {
    //     { (0, 2), (4, 5) },
    //     { (0, 6), (7, 2) }
    //};

    // public static readonly int[,] ObstacleLayout = new int[,] //medium
    // {
    //     {0,0,0,0,0,0,0,0,0,0,0,0},
    //     {0,1,1,1,0,1,1,0,1,1,3,0},
    //     {0,1,0,1,0,1,1,0,1,0,1,0},
    //     {0,1,0,1,1,1,1,1,1,0,1,0},
    //     {0,1,0,0,0,1,1,0,0,0,1,0},
    //     {0,1,1,1,0,1,1,0,1,1,1,0},
    //     {0,0,0,1,0,0,0,0,1,0,0,0},
    //     {0,1,1,1,0,1,1,0,1,1,1,0},
    //     {0,1,0,0,0,1,1,0,0,0,1,0},
    //     {0,1,0,1,1,1,1,1,1,0,1,0},
    //     {0,1,0,1,0,1,1,0,1,0,1,0},
    //     {0,2,1,1,0,1,1,0,1,1,1,0},
    //     {0,0,0,0,0,0,0,0,0,0,0,0}
    // };

    public static readonly int[,] EasyMazeTestingLayout = new int[,]
    {
        {1,1,5,1,1,1,5,1,1,3},
        {1,0,0,0,0,0,0,0,0,1},
        {1,0,1,1,1,1,1,1,0,1},
        {1,0,1,0,0,0,0,1,0,1},
        {12,0,1,0,1,6,0,1,0,1},
        {1,0,1,0,1,1,0,1,0,1},
        {1,0,1,0,0,0,0,1,0,1},
        {1,0,6,1,1,1,1,1,0,1},
        {1,0,0,0,0,0,0,0,0,1},
        {2,7,8,1,1,12,1,13,1,1}
    };

    public static readonly Dictionary<(int, int), (int, int)> EasyMazeTestingConnections = new Dictionary<(int, int), (int, int)>
    {
        { (0, 2), (4, 5) },
        { (0, 6), (7, 2) },
        { (4, 0), (6, 0) },
        { (9, 5), (9, 7) }
    };

    public static readonly Dictionary<(int, int), int> EasyMazeTestingEnemyTypes = new Dictionary<(int, int), int>
    {
        { (4, 0), 0},
        { (9, 5), 1}
    };

    public static readonly Dictionary<(int, int), int> EasyMazeTestingEnemyRewardTypes = new Dictionary<(int, int), int>
    {
        { (4, 0), 0 },
        { (9, 5), 1}
    };


    public static readonly int[,] ObstacleLayout = new int[,] //medium
    {
        {3,1,0,0,0,0,0,0,0,0,11,2},
        {0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0},
        {0,11,0,11,0,0,11,0,0,0,11,0}
    };

    // public static readonly int[,] FireLayout = new int[,]
    // {
    //     {0,0,1,1,1,1,1,1,1,0,0,0,0,0},
    //     {0,0,1,1,0,0,1,1,1,0,0,1,1,1},
    //     {1,1,1,1,0,0,1,1,1,1,1,1,3,1},
    //     {1,1,1,1,0,0,0,0,0,0,0,1,1,1},
    //     {1,2,1,1,0,0,0,0,0,0,0,1,1,1},
    //     {1,1,1,0,0,0,0,0,0,0,0,0,0,0}
    // };

    public static readonly int[,] FireLayout = new int[,]
    {
        {3,0,0,0,0,0,0,0,0,0,0,0,0,5,2},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {4,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {4,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {4,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,5,6,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {4,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {4,6,0,0,0,0,5,0,0,0,0,0,5,0,0}
    };

    public static readonly Dictionary<(int, int), (int, int)> FireLayoutConnections = new Dictionary<(int, int), (int, int)>
    {
        { (0, 13), (9, 13) },
        { (9, 12), (14, 12) },
        { (14, 12), (14, 6) },
        { (14, 6), (14, 1) }
    };


    public static readonly int[,] FireTestingLayout = new int[,]
    {
        {0,0,1,1,1,1,1,1,1,0,0,0,0,0},
        {0,0,1,1,0,0,1,1,1,0,0,1,1,1},
        {1,1,1,1,0,0,1,1,1,1,1,1,3,1},
        {1,1,1,1,0,0,0,0,0,0,0,1,1,1},
        {1,2,1,13,0,0,0,0,0,0,0,1,1,1},
        {1,1,1,0,0,0,0,0,0,0,0,0,0,0}
    };

    public static readonly Dictionary<(int, int), (int, int)[]> FireTestingLayoutTrigger = new Dictionary<(int, int), (int, int)[]>
    {
        { (4, 3), new (int, int)[] { (4, 5), (4, 7), (4, 9) } }
    };

    public static readonly Dictionary<(int, int), (int, int)> FireTestingLayoutConnections = new Dictionary<(int, int), (int, int)>
    {
        { (4, 5), (4, 7) },
        { (4, 7), (4, 9) },
        { (4, 9), (4, 11) }
    };

    public static readonly int[,] FireTesting2Layout = new int[,]
    {
        {0,0,1,1,1,1,1,1,1,0,0,0,0,0},
        {0,0,1,1,0,0,1,1,1,0,0,1,1,1},
        {1,1,1,1,0,0,1,1,1,1,1,1,3,1},
        {1,1,1,1,0,0,0,0,0,0,0,1,1,1},
        {1,2,1,14,15,15,15,15,15,15,15,1,1,1},
        {1,1,1,0,0,0,0,0,0,0,0,0,0,0}
    };

    public static readonly Dictionary<(int, int), (int, int)[]> FireTesting2LayoutTrigger = new Dictionary<(int, int), (int, int)[]>
    {
        { (4, 3), new (int, int)[] { (4, 4), (4, 5), (4, 6), (4, 7), (4, 8), (4, 9), (4, 10) } }
    };


    public static readonly int[,] MixedPathLayout = new int[,]
    {
        {0,0,0,0,1,1,1,0,0,0,0,0},
        {0,2,1,1,1,0,1,0,0,1,1,0},
        {0,0,0,0,1,0,1,1,1,1,0,0},
        {0,1,1,1,1,0,0,0,0,1,0,0},
        {0,1,0,0,1,1,1,1,0,1,1,0},
        {0,1,0,0,0,0,0,1,0,0,1,0},
        {0,1,1,1,1,1,1,1,0,0,1,0},
        {0,0,0,0,0,0,0,1,1,1,1,0},
        {0,1,1,1,0,0,0,0,0,0,3,0},
        {0,0,0,1,1,1,1,1,1,1,1,0}
    };

    public static readonly int[,] IslandPathLayout = new int[,]
    {
        {0,0,0,0,0,1,1,1,0,0,0,0},
        {0,2,1,1,1,1,0,1,0,0,0,0},
        {0,0,0,0,1,0,0,1,1,1,0,0},
        {0,0,1,1,1,0,0,0,0,1,0,0},
        {0,0,1,0,0,0,1,1,1,1,0,0},
        {0,0,1,0,0,0,1,0,0,0,0,0},
        {0,0,1,1,1,1,1,0,1,1,1,0},
        {0,0,0,0,0,1,0,0,1,3,1,0},
        {0,0,0,0,0,1,1,1,1,1,1,0}
    };

    public static readonly int[,] SpiralPathLayout = new int[,]
    {
        {2,1,1,1,1,1,1,1,1,1},
        {0,0,0,0,0,0,0,0,0,1},
        {0,1,1,1,1,1,1,1,0,1},
        {0,1,0,0,0,0,0,1,0,1},
        {0,1,0,1,1,1,0,1,0,1},
        {0,1,0,1,3,1,0,1,0,1},
        {0,1,0,1,1,1,0,1,0,1},
        {0,1,0,0,0,0,0,1,0,1},
        {0,1,1,1,1,1,1,1,0,1},
        {0,0,0,0,0,0,0,0,0,1}
    };

    public static readonly int[,] ScatteredIslandsLayout = new int[,]
    {
        {0,0,1,0,0,0,1,1,0,0,0,1},
        {0,1,1,1,0,0,1,3,1,0,0,1},
        {1,1,2,1,0,0,0,1,0,0,0,0},
        {0,1,1,1,0,0,0,1,1,1,0,0},
        {0,0,1,0,0,1,1,0,0,1,0,0},
        {0,0,0,0,0,1,0,0,0,1,1,0},
        {0,1,1,1,0,1,1,1,0,0,1,0},
        {0,1,0,1,0,0,0,1,0,0,1,0},
        {0,1,1,1,0,0,0,1,1,1,1,0}
    };

    public static readonly int[,] AdvancedLayout = new int[,]
    {
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,0,0,0,2},
        {0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,11,1,1,1},
        {0,0,0,0,1,1,1,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,1,7,1},
        {0,0,0,1,1,0,1,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,7,1,0,0,1,1,1},
        {0,0,0,1,0,0,1,1,0,0,0,1,0,0,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0},
        {0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,1,1,1,1,1,0,1,0,0,0,1,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,1,0,0,1,1,1,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,1,1,0,0,0,1,1,0,0,0,0,1,1,0,0,0,0,0,1,1,0,0,0,0,4,0,0,0},
        {0,1,1,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0},
        {1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,1,1,0},
        {0,0,1,1,1,0,0,0,0,0,0,0,1,5,1,1,0,0,1,1,0,0,0,0,0,1,1,0,0,0,0,1,1,1,1},
        {0,0,0,0,1,0,0,0,0,0,0,0,1,1,1,0,0,0,0,1,1,0,0,0,1,1,0,0,0,0,1,1,0,0,1},
        {0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,1,1,1,4,1,0,0,1,0,0,0,0,1,1,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,0,0,0},
        {0,0,0,0,0,0,0,1,1,1,0,4,1,1,1,1,1,1,0,0,1,1,1,0,0,1,1,1,1,0,0,0,0,0,0},
        {0,0,0,0,0,0,1,1,0,1,0,0,0,0,0,0,1,1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0},
        {0,0,0,0,0,1,1,0,0,1,0,0,0,1,0,0,1,1,1,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,1,1,1,1,0,0,0,1,0,0,1,0,0,0,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,1,0,0,1,0,0,0,1,0,0,0,1,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {1,1,0,0,0,1,1,0,1,1,0,0,0,0,1,0,0,1,0,0,0,1,1,0,0,0,0,0,0,1,0,1,0,0,0},
        {1,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,1,0,1,0,0,0},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,0,0,1,1,0,0},
        {0,1,1,0,0,0,0,0,0,0,0,0,0,1,1,0,0,1,1,1,0,0,1,1,1,0,1,0,0,0,0,0,1,0,0},
        {0,0,1,0,0,0,0,0,0,0,0,1,0,1,1,0,0,0,1,1,1,0,1,6,1,1,0,0,0,0,0,0,0,0,0},
        {0,0,1,1,0,0,1,0,0,0,1,1,0,1,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,1,0,0,1,1,0,0,1,4,1,1,0,0,0,1,1,0,0,1,1,1,0,0,0,0,0,1,1,1,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,0,1,1,0,0},
        {0,1,0,0,1,1,0,1,1,0,0,0,0,0,0,0,0,1,0,0,0,0,1,0,1,0,0,0,0,0,0,0,1,0,0},
        {1,1,1,0,0,1,1,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,1,1,0},
        {1,1,1,0,0,0,0,0,0,1,1,0,0,0,1,1,0,1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,1,0},
        {0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,1,0,1,1,0,0,0,0,0,0},
        {0,0,1,0,0,1,0,0,1,0,0,1,1,0,0,0,0,0,1,1,1,0,0,0,0,1,0,1,1,0,8,0,8,0,0},
        {11,0,0,0,0,1,1,0,0,0,0,1,0,0,0,1,1,0,0,0,1,0,1,1,1,0,0,0,0,0,8,8,0,0,0},
        {0,0,9,0,0,0,0,0,0,1,1,1,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,1,1,0,1,1,0,0},
        {3,0,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,0,1,1,7,7,1,1,0,0}
    };

    public static readonly Dictionary<(int, int), (float, float, float, float)> AdvancedLayoutMoving = new Dictionary<(int, int), (float, float, float, float)>
    {
        { (33, 2), (0f, 0f, 2f, 2f) },
        { (0, 30), (0f, 0f, 2f, 2f) }
    };

}
