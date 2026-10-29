using TMPro;
using UnityEngine;

/// <summary>Keeps a text showing the star balance, e.g. "kopa: {0}".</summary>
public class StarBalanceLabel : MonoBehaviour
{
    public TMP_Text label;
    public string format = "{0}";

    private void OnEnable()
    {
        StarBank.Changed += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        StarBank.Changed -= Refresh;
    }

    private void Refresh()
    {
        if (label != null)
            label.text = string.Format(format, StarBank.Balance);
    }
}
