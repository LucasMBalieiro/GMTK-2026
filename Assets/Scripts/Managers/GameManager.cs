using Entities;
using Level;
using UnityEngine;
using UnityUtils;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private EntityData playerStats;
    
    public EntityData PlayerStats => playerStats;

    public LevelData LevelData { get; private set; }

    public void SetLevelData(LevelData levelData)
    {
        LevelData = levelData;
    }
}
