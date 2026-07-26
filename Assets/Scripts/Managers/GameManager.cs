using System.Collections.Generic;
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
    private readonly List<int> mapProgress = new List<int>();
    public IReadOnlyList<int> MapProgress => mapProgress;

    public void SetLevelData(LevelData levelData)
    {
        LevelData = levelData;
    }

    /// <summary>
    /// Guarda o caminho de nós já visitados no mapa atual, para o MapVisualController
    /// restaurar isso ao recarregar a cena do Mapa (ex: voltando do Combate).
    /// </summary>
    public void SetMapProgress(IEnumerable<int> visitedNodeIds)
    {
        mapProgress.Clear();
        mapProgress.AddRange(visitedNodeIds);
    }

    /// <summary>
    /// Zera o progresso do mapa. Chamar ao iniciar uma run nova (ex: botão Jogar do Menu).
    /// </summary>
    public void ResetMapProgress()
    {
        mapProgress.Clear();
    }

    /// <summary>
    /// Sorteia uma das fases possíveis do pool, guarda o LevelData escolhido
    /// (para a cena de combate consultar depois) e carrega a cena correspondente.
    /// </summary>
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