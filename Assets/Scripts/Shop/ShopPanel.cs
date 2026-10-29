using TMPro;
using UnityEngine;

/// <summary>Shop popup in the start menu, opened from the phone on the dashboard.</summary>
public class ShopPanel : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text balanceText;

    private void OnEnable()
    {
        StarBank.Changed += UpdateBalance;
        UpdateBalance();
    }

    private void OnDisable()
    {
        StarBank.Changed -= UpdateBalance;
    }

    public void Open()
    {
        panel.SetActive(true);
        UpdateBalance();
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    private void UpdateBalance()
    {
        if (balanceText != null)
            balanceText.text = StarBank.Balance.ToString();
    }
}
