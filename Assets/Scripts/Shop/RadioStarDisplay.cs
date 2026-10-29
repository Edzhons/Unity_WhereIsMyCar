using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>The car radio's display shows the star balance instead of "CITY FM".</summary>
public class RadioStarDisplay : MonoBehaviour
{
    public TMP_Text displayText;
    public float messageTime = 1.5f;

    private Coroutine messageRoutine;

    private void OnEnable()
    {
        StarBank.Changed += ShowBalance;
        ShowBalance();
    }

    private void OnDisable()
    {
        StarBank.Changed -= ShowBalance;
    }

    public void ShowBalance()
    {
        if (messageRoutine == null && displayText != null)
            displayText.text = StarBank.Balance.ToString();
    }

    /// <summary>Shows a short message (like "+50") and then the balance again.</summary>
    public void ShowMessage(string message)
    {
        if (messageRoutine != null)
            StopCoroutine(messageRoutine);
        messageRoutine = StartCoroutine(Message(message));
    }

    IEnumerator Message(string message)
    {
        displayText.text = message;
        yield return new WaitForSecondsRealtime(messageTime);
        messageRoutine = null;
        ShowBalance();
    }
}
