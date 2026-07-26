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

    [SerializeField] private ModalScriptableObject tutorial1;
    [SerializeField] private ModalScriptableObject tutorial2;
    [SerializeField] private ModalScriptableObject tutorial2_2;
    
    [SerializeField] private ModalScriptableObject tutorial3;
    

    private void Awake()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    // Ligar no botão "Jogar"
    public void OnPlayPressed()
    {
        if (string.IsNullOrEmpty(mapSceneName))
        {
            Debug.LogWarning("MainMenuController: Map Scene Name não configurado.");
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetMapProgress();
            GameManager.Instance.RestartPlayer();
        }

        SceneManager.LoadScene(mapSceneName);
    }

    public void OpenTutorial()
    {
        ModalManager.Instance.ShowModal(tutorial1, onConfirm: OpenTutorial2, autoCloseConfirm: false);
    }

    private void OpenTutorial2()
    {
        ModalManager.Instance.ShowModal(tutorial2, onConfirm: OpenTutorial2_2, onCancel: OpenTutorial, autoCloseConfirm: false, autoCloseCancel: false);
    }

    private void OpenTutorial2_2()
    {
        ModalManager.Instance.ShowModal(tutorial2_2, onConfirm: OpenTutorial3, onCancel: OpenTutorial2, autoCloseConfirm: false, autoCloseCancel: false);
    }

    private void OpenTutorial3()
    {
        ModalManager.Instance.ShowModal(tutorial3, onCancel: OpenTutorial2_2, autoCloseCancel: false);
    }

    // Ligar no botão "Opções"
    public void OnOptionsPressed()
    {
        if (optionsPanel == null)
        {
            Debug.LogWarning("MainMenuController: Options Panel não atribuído.");
            return;
        }
        optionsPanel.SetActive(true);
    }

    // Ligar no botão "Fechar"/"Voltar" dentro do painel de Opções
    public void OnCloseOptionsPressed()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    // Ligar no botão "Sair"
    public void OnQuitPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}