using UnityEngine;

public class TitleAnimation : MonoBehaviour
{
    public float moveAmount = 5f;
    public float speed = 1f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        float movement = Mathf.Sin(Time.time * speed) * moveAmount;

        transform.localPosition =
            startPosition + new Vector3(0f, movement, 0f);
    }
}