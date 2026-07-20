using UnityEngine;
using System;

public class ConfirmPurchaseUI : MonoBehaviour
{
    [SerializeField] private GameObject confirmPanel;

    private Action onConfirm;
    private Action onCancel;

    public void Show(int cost, Action confirmCallback, Action cancelCallback) //show confirm panel. 
    {
        onConfirm = confirmCallback;
        onCancel = cancelCallback;
        confirmPanel.SetActive(true);
    }

    public void OnConfirmClicked() // confirmed.
    {
        confirmPanel.SetActive(false);
        onConfirm?.Invoke();
    }

    public void OnCancelClicked() //cancel.
    {
        confirmPanel.SetActive(false);
        onCancel?.Invoke();
    }
}