using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Navegação")]
    [Tooltip("Nome exato da cena do Mapa (deve estar em Build Settings > Scenes In Build)")]
    [SerializeField] private string mapSceneName;

    [Header("Opções")]
    [Tooltip("Painel de Opções, desativado por padrão. Arraste o GameObject aqui.")]
    [SerializeField] private GameObject optionsPanel;

    private void Awake()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    public void OnPlayPressed()
    {
        if (string.IsNullOrEmpty(mapSceneName))
        {
            Debug.LogWarning("MainMenuController: Map Scene Name não configurado.");
            return;
        }
        SceneManager.LoadScene(mapSceneName);
    }


    public void OnOptionsPressed()
    {
        if (optionsPanel == null)
        {
            Debug.LogWarning("MainMenuController: Options Panel não atribuído.");
            return;
        }
        optionsPanel.SetActive(true);
    }

 
    public void OnCloseOptionsPressed()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

  
    public void OnQuitPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}