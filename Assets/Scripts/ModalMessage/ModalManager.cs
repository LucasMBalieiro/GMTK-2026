using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModalManager : MonoBehaviour
{
    #region UIElements

        [Header("UI Elements")]
        [SerializeField] private RectTransform modalPanel;
        
        [SerializeField] private GameObject headerGameObject;
        [SerializeField] private GameObject contentGameObject;
        [SerializeField] private GameObject footerGameObject;

        [Header("Header")]
        [SerializeField] private TextMeshProUGUI titleText;
        

        [Header("Horizontal Layout")]
        [SerializeField] private GameObject horizontalLayoutGameObject;
        [SerializeField] private GameObject horizontalImageGameObject;
        [SerializeField] private Image horizontalImage;
        [SerializeField] private TextMeshProUGUI horizontalMessageText;
        
        [Header("Vertical Layout")]
        [SerializeField] private GameObject verticalLayoutGameObject;
        [SerializeField] private GameObject verticalImageGameObject;
        [SerializeField] private Image verticalImage;
        [SerializeField] private TextMeshProUGUI verticalMessageText;
        
        [Header("Footer")]
        [SerializeField] private Button confirmButton;
        [SerializeField] private TextMeshProUGUI confirmButtonText;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TextMeshProUGUI cancelButtonText;
        [SerializeField] private Button altButton;
        [SerializeField] private TextMeshProUGUI alternativeMessageText;

    #endregion

    #region Singleton

        public static ModalManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            
            Instance = this;
            DontDestroyOnLoad(gameObject);

            CheckEventSystem();
            CloseModal();
        }
        
        
        /*
         *  Essa checkagem não é necessária, ela serve apenas para garantir que o sistema funcione sem o usuário precisar
         *  saber como funciona um botão na Unity (precisa de um EventSystem na cena para você clickar com a tela)
         *
         *  Caso você garanta que todas as cenas do seu jogo possuem um (se todas tiverem canvas, já vão ter) sinta-se livre
         *  para deletar esse método.
         */
        private void CheckEventSystem()
        {
            EventSystem currentEventSystem = FindAnyObjectByType<EventSystem>();

            if (currentEventSystem == null)
            {
                GameObject eventSystemGO = new GameObject("EventSystem (Auto-Generated)");
                eventSystemGO.AddComponent<EventSystem>();
                
                Debug.LogWarning("ModalManager: EventSystem não encontrado na hierarquia, gerando um automaticamente");

                #if ENABLE_INPUT_SYSTEM
                    eventSystemGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                #else
                eventSystemGO.AddComponent<StandaloneInputModule>();
                #endif
                    DontDestroyOnLoad(eventSystemGO);
            }
        }

    #endregion
    
    public void ShowModal(ModalDetails modalDetails, bool autoCloseConfirm = true,  bool autoCloseCancel = true, bool autoCloseAlt = true)
    {
        modalPanel.gameObject.SetActive(true);
        
        ConfigureHeader(modalDetails.title);
        
        ConfigureLayout(modalDetails);
        
        ConfigureButton(confirmButton, confirmButtonText, modalDetails.confirmText, modalDetails.onConfirm, autoCloseConfirm);
        ConfigureButton(cancelButton, cancelButtonText, modalDetails.cancelText, modalDetails.onCancel, autoCloseCancel);
        ConfigureButton(altButton, alternativeMessageText, modalDetails.alternativeText, modalDetails.onAlternative, autoCloseAlt);
        
        if (titleText.isActiveAndEnabled) titleText.ForceMeshUpdate();
        if (horizontalMessageText.isActiveAndEnabled) horizontalMessageText.ForceMeshUpdate();
        if (verticalMessageText.isActiveAndEnabled) verticalMessageText.ForceMeshUpdate();
        
        Canvas.ForceUpdateCanvases();
        
        ForceDeepRebuild(modalPanel);
    }
    
    public void ShowModal(ModalScriptableObject modalScriptableObject, Action onConfirm = null, Action onCancel = null, Action onAlternative = null, bool autoCloseConfirm = true, bool autoCloseCancel = true, bool autoCloseAlt = true)
    {
        ModalDetails modalDetails = new ModalDetails(modalScriptableObject, onConfirm, onCancel, onAlternative);
        
        ShowModal(modalDetails, autoCloseConfirm, autoCloseCancel, autoCloseAlt);
    }

    private void ConfigureHeader(string titleDetails)
    {
        if (!string.IsNullOrEmpty(titleDetails))
        {
            headerGameObject.SetActive(true);
            titleText.text = titleDetails;
        }
        else
        {
            headerGameObject.SetActive(false);
        }
    }

    private void ConfigureLayout(ModalDetails modalDetails)
    {
        horizontalLayoutGameObject.SetActive(false);
        verticalLayoutGameObject.SetActive(false);

        switch (modalDetails.layout)
        {
            case ModalLayout.Horizontal:
            {
                horizontalLayoutGameObject.SetActive(true);
                horizontalMessageText.text = modalDetails.message;

                if (modalDetails.contentImage)
                {
                    horizontalImageGameObject.SetActive(true);
                    horizontalImage.sprite = modalDetails.contentImage;
                }
                else
                {
                    horizontalImageGameObject.SetActive(false);
                }

                break;
            }
            case ModalLayout.Vertical:
            {
                verticalLayoutGameObject.SetActive(true);
                verticalMessageText.text = modalDetails.message;

                if (modalDetails.contentImage)
                {
                    verticalImageGameObject.SetActive(true);
                    verticalImage.sprite = modalDetails.contentImage;
                }
                else
                {
                    verticalImageGameObject.SetActive(false);
                }
                
                if(modalDetails.message == null &&  !modalDetails.contentImage) contentGameObject.SetActive(false);

                break;
            }
        }
    }
    
    private void ConfigureButton(Button button, TextMeshProUGUI textContainer, string contentText, Action buttonAction, bool autoClose)
    {
        if (!string.IsNullOrEmpty(contentText))
        {
            button.gameObject.SetActive(true);
            textContainer.text = contentText;
            
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                buttonAction?.Invoke();
                if(autoClose) CloseModal();
            });
        }
        else
        {
            button.gameObject.SetActive(false);
        }
    }
    
    private void ForceDeepRebuild(RectTransform target)
    {
        foreach (RectTransform child in target)
        {
            if (child.gameObject.activeSelf) LayoutRebuilder.ForceRebuildLayoutImmediate(child);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(target);
    }
    
    public void CloseModal()
    {
        modalPanel.gameObject.SetActive(false);
    }
}
