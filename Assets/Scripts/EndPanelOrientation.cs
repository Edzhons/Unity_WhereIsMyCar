using UnityEngine;

/// <summary>
/// End panel buttons: side by side when the phone is held sideways,
/// one under the other when it is upright.
/// </summary>
public class EndPanelOrientation : MonoBehaviour
{
    public RectTransform menuButton;
    public RectTransform restartButton;

    public Vector2 sideBySideMenu = new Vector2(-230, -210);
    public Vector2 sideBySideRestart = new Vector2(230, -210);
    public Vector2 stackedMenu = new Vector2(0, -330);
    public Vector2 stackedRestart = new Vector2(0, -210);

    private void Update()
    {
        bool upright = Screen.height > Screen.width;
        menuButton.anchoredPosition = upright ? stackedMenu : sideBySideMenu;
        restartButton.anchoredPosition = upright ? stackedRestart : sideBySideRestart;
    }
}
