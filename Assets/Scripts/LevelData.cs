using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Level", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public Vector3 playerStartPosition;
    public List<PlatformData> platforms = new List<PlatformData>();
    public float timeLimit = 60f;
    public int checkpointIndex = -1;

    [Serializable]
    public class PlatformData
    {
        public Vector3 position;
        public Vector3 scale = Vector3.one;
        public Color color = Color.white;
        public string directionTag;
    }

    public enum PlatformType
    {
        Normal,
        Reward,
        Spring,
        Hidden,
        Explosive,
        Checkpoint,
        Goal
    }
}