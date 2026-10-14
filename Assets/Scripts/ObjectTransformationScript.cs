using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectTransformationScript : MonoBehaviour
{
    public float minSize = 0.6f;
    public float maxSize = 1.3f;
    public float sizeSpeed = 0.003f;

    public float scaleSpeed = 0.5f;
    public float rotationSpeed = 90f;
    private bool rotateCW, rotateCCW, scaleUpY, scaleDownY, scaleUpX, scaleDownX, isFlipped;
    public static bool isTransforming = false;

    void Update()
    {
        if (GameObjectsScript.lastDragged == null)
        {
            return;
        }

        RectTransform rc = GameObjectsScript.lastDragged.GetComponent<RectTransform>();
        if (rotateCW)
        {
            rc.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
        }

        if (rotateCCW)
        {
            rc.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }

        if (scaleUpY && rc.localScale.y < maxSize)
        {
            rc.localScale += new Vector3(0, scaleSpeed * Time.deltaTime, 0);
        }

        if (scaleDownY && rc.localScale.y > minSize)
        {
            rc.localScale -= new Vector3(0, scaleSpeed * Time.deltaTime, 0);
        }

        if (scaleUpX && Mathf.Abs(rc.localScale.x) < maxSize)
        {
            float sign = Mathf.Sign(rc.localScale.x);
            rc.localScale = new Vector3(
                sign * Mathf.Min(Mathf.Abs(rc.localScale.x) + scaleSpeed * Time.deltaTime, maxSize),
                rc.localScale.y,
                rc.localScale.z
            );
        }

        if (scaleDownX && Mathf.Abs(rc.localScale.x) > minSize)
        {
            float sign = Mathf.Sign(rc.localScale.x);
            rc.localScale = new Vector3(
                sign * Mathf.Max(Mathf.Abs(rc.localScale.x) - scaleSpeed * Time.deltaTime, minSize),
                rc.localScale.y,
                rc.localScale.z
            );
        }

        isTransforming = rotateCW || rotateCCW || scaleUpY || scaleDownY || scaleUpX || scaleDownX;
    }

    public void StartRotateCW(BaseEventData eventData) { rotateCW = true; }

    public void StopRotateCW(BaseEventData eventData) { rotateCW = false; }


    public void StartRotateCCW(BaseEventData eventData) { rotateCCW = true; }

    public void StopRotateCCW(BaseEventData eventData) { rotateCCW = false; }


    public void StartScaleUpY(BaseEventData eventData) { scaleUpY = true; }
    public void StopScaleUpY(BaseEventData eventData) { scaleUpY = false; }
    public void StartScaleDownY(BaseEventData eventData) { scaleDownY = true; }
    public void StopScaleDownY(BaseEventData eventData) { scaleDownY = false; }

    public void StartScaleUpX(BaseEventData eventData) { scaleUpX = true; }
    public void StopScaleUpX(BaseEventData eventData) { scaleUpX = false; }
    public void StartScaleDownX(BaseEventData eventData) { scaleDownX = true; }
    public void StopScaleDownX(BaseEventData eventData) { scaleDownX = false; }

    public void FlipCar()
    {
        if (GameObjectsScript.lastDragged == null)
            return;

        RectTransform rc = GameObjectsScript.lastDragged.GetComponent<RectTransform>();

        Vector3 scale = rc.localScale;
        scale.x *= -1f;
        rc.localScale = scale;

        isFlipped = !isFlipped;
    }
}