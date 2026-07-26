using Entities;
using Level;
using UnityEngine;
using UnityUtils;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private EntityData playerStats;
    
    public EntityData PlayerStats => playerStats;

    [SerializeField] private LevelData DEBUG;
    
    
    public LevelData LevelData { get; private set; }

    private new void Awake()
    {
        LevelData = DEBUG;
    }
    
    public void SetLevelData(LevelData levelData)
    {
        LevelData = levelData;
    }
}
