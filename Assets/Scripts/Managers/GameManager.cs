using Entities;
using Level;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public void StartLevel(LevelPool pool)
    {
        if (pool == null || pool.phases == null || pool.phases.Count == 0)
        {
            Debug.LogWarning("StartLevel chamado com um LevelPool vazio ou nulo.");
            return;
        }

        int index = Random.Range(0, pool.phases.Count);
        var phase = pool.phases[index];

        if (string.IsNullOrEmpty(phase.SceneName))
        {
            Debug.LogWarning($"A fase sorteada (índice {index}) não tem SceneName definido.");
            return;
        }

        SetLevelData(phase.Data);
        SceneManager.LoadScene(phase.SceneName);
    }
}