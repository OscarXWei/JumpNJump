using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Level", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public Vector3 playerStartPosition;
    public List<PlatformData> platforms = new List<PlatformData>();
    public Dictionary<GameObject, (int, int)> platformPositions = new Dictionary<GameObject, (int, int)>();
    public Dictionary<(int, int), GameObject> positionPlatforms = new Dictionary<(int, int), GameObject>();
    public Dictionary<(int, int), (int, int)> platformsConnections = new Dictionary<(int, int), (int, int)>();
    public Dictionary<(int, int), int> platformsEnemyTypes = new Dictionary<(int, int), int>();
    public Dictionary<(int, int), int> platformsEnemyRewardTypes = new Dictionary<(int, int), int>();
    public Dictionary<(int, int), (int, int)[]> springsTrigger = new Dictionary<(int, int), (int, int)[]>();
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
        
        public bool isMoving;
        public Vector3 moveStart;
        public Vector3 moveEnd;
        public float moveDuration = 2f;
        public int zInArray = -1;
        public int xInArray = -1;
    }

    public enum PlatformType
    {
        Empty,
        Start,
        Normal,
        Reward,
        SpringStart,
        SprintEnd,
        Hidden,
        Explosive,
        Checkpoint,
        Moving,
        CoinTrigger,
        Coin,
        Goal,
        Invincible,
        EnemySrc,
        SpringsTrigger,
        SpringTemp
    }
}
