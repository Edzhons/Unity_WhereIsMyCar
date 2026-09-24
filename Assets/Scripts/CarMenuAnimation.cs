using UnityEngine;

public class CarMenuAnimation : MonoBehaviour
{
    public float moveAmount = 10f;
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
            startPosition + new Vector3(movement, 0f, 0f);
    }
}