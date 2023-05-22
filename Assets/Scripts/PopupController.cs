using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    static PopupController instance;

    [SerializeField] UnityEngine.UI.Text text;
    [SerializeField] GameObject popup;
    private System.Action onConfirmation;

    private void Awake()
    {
        instance = this;
    }

    public static void MakePopup(string message, System.Action action)
    {
        instance.Popup(message, action);
    }
    void Popup(string message, System.Action action)
    {
        popup.SetActive(true);
        text.text = message;
        onConfirmation = action;
    }

    public void OnConfirmation()
    {
        popup.SetActive(false);
        if (onConfirmation != null) onConfirmation.Invoke();
    }

}
