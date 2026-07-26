using AudioSystem;
using Entities;
using Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtils;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EntityData playerStats;
    [SerializeField] private SoundData musicData;
    
    public EntityData PlayerStats => playerStats;
    public LevelData LevelData { get; private set; }

    public static GameManager Instance { get; private set; }
    
    private SoundEmitter musicEmitter;
    private bool musicPlaying;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMusic();
    }

    private void PlayMusic()
    {
        musicEmitter = SoundManager.Instance.CreateSound().PlayOnSoundEmitter(musicData);
    }

    public void ToggleMusic()
    {
        if (musicPlaying)
        {
            musicEmitter.Stop();
            musicPlaying = false;
        }
        else
        {
            musicEmitter.Play();
            musicPlaying = true;
        }
    }

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