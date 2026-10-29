using UnityEngine;

/// <summary>Slowly grows and shrinks, so the player notices something can be tapped.</summary>
public class GentlePulse : MonoBehaviour
{
    public float amount = 0.06f;
    public float speed = 2.5f;

    private void Update()
    {
        float scale = 1f + Mathf.Sin(Time.unscaledTime * speed) * amount;
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
