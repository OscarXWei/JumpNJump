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
        public PlatformType type;

        public bool isKinematic;
        public bool useGravity;
        public bool freezePosition;
        public bool freezeRotation;
    }

    public enum PlatformType
    {
        Start,
        Normal,
        Reward,
        Spring,
        Hidden,
        Explosive,
        Checkpoint,
        Moving,
        Goal
    }
}