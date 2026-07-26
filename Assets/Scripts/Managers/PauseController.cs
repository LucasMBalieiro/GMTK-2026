using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityUtils;

public class PauseController : Singleton<PauseController>
{
    [Header("UI")]
    [Tooltip("Painel raiz do menu de pause (Continuar / Opções / Voltar pro Menu)")]
    [SerializeField] private GameObject pauseMenuRoot;
    [Tooltip("Painel de Opções (sliders de volume), aberto a partir do menu de pause")]
    [SerializeField] private GameObject optionsPanel;

    [Header("Navegação")]
    [Tooltip("Nome exato da cena do Menu Principal (deve estar em Build Settings > Scenes In Build)")]
    [SerializeField] private string mainMenuSceneName;

    public bool IsPaused { get; private set; }

    private void Start()
    {
        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        Time.timeScale = 1f;
        IsPaused = false;
        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }
    public void TogglePause()
    {
        if (IsPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (IsPaused) return;
        IsPaused = true;
        Time.timeScale = 0f;
        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(true);
    }
    public void Resume()
    {
        if (!IsPaused) return;
        IsPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    public void OnOptionsPressed()
    {
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void OnCloseOptionsPressed()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    public void OnReturnToMenuPressed()
    {
        if (string.IsNullOrEmpty(mainMenuSceneName))
        {
            Debug.LogWarning("PauseController: Main Menu Scene Name não configurado.");
            return;
        }

        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}