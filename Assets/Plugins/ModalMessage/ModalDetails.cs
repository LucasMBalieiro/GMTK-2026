using System;
using UnityEngine;

public enum ModalLayout
{
    Vertical,
    Horizontal
}

public class ModalDetails
{
    public string title;
    public string message;
    public Sprite contentImage;
    public ModalLayout layout = ModalLayout.Vertical;

    public string confirmText = "Confirmar";
    public Action onConfirm;

    public string cancelText; 
    public Action onCancel;

    public string alternativeText;
    public Action onAlternative;
    
    public ModalDetails() { } 

    public ModalDetails(ModalScriptableObject template, Action onConfirm = null, Action onCancel = null, Action onAlternative = null)
    {
        title = template.title;
        message = template.message;
        contentImage = template.contentImage;
        layout = template.layout;
        
        confirmText = template.confirmText;
        cancelText = template.cancelText;
        alternativeText = template.alternativeText;

        this.onConfirm = onConfirm;
        this.onCancel = onCancel;
        this.onAlternative = onAlternative;
    }
}