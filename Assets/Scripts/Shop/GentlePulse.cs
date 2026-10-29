using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Slowly grows and shrinks, so the player notices something can be tapped.
/// With a fade target it pulses that graphic's transparency instead.
/// </summary>
public class GentlePulse : MonoBehaviour
{
    public float amount = 0.06f;
    public float speed = 2.5f;

    [Tooltip("Optional: pulse this graphic's alpha instead of the scale.")]
    public Graphic fadeTarget;
    public float minAlpha = 0.15f;
    public float maxAlpha = 0.85f;

    private void Update()
    {
        float wave = Mathf.Sin(Time.unscaledTime * speed);

        if (fadeTarget != null)
        {
            Color color = fadeTarget.color;
            color.a = Mathf.Lerp(minAlpha, maxAlpha, wave * 0.5f + 0.5f);
            fadeTarget.color = color;
            return;
        }

        float scale = 1f + wave * amount;
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
