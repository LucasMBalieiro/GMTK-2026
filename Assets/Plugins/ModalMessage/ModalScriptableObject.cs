using UnityEngine;

[CreateAssetMenu(fileName = "ModalScriptableObject", menuName = "Modal Message/Modal Details")]
public class ModalScriptableObject: ScriptableObject
{
    [Header("Header & Content")]
    public string title;
    [TextArea(3, 10)]
    public string message;
    public Sprite contentImage;
    public ModalLayout layout = ModalLayout.Vertical;
    
    [Header("Footer")]
    public string confirmText = "Confirmar";
    public string cancelText;
    public string alternativeText;
}
