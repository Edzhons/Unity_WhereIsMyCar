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

        // Keyboard (see README): Z/X rotate, arrows change size, Space mirrors.
        bool keyCCW = Input.GetKey(KeyCode.Z);
        bool keyCW = Input.GetKey(KeyCode.X);
        bool keyTaller = Input.GetKey(KeyCode.UpArrow);
        bool keyShorter = Input.GetKey(KeyCode.DownArrow);
        bool keyWider = Input.GetKey(KeyCode.RightArrow);
        bool keyNarrower = Input.GetKey(KeyCode.LeftArrow);
        if (Input.GetKeyDown(KeyCode.Space))
            FlipCar();

        if (rotateCW || keyCW)
        {
            rc.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
        }

        if (rotateCCW || keyCCW)
        {
            rc.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }

        if ((scaleUpY || keyTaller) && rc.localScale.y < maxSize)
        {
            rc.localScale = new Vector3(rc.localScale.x, Mathf.Min(rc.localScale.y + scaleSpeed * Time.deltaTime, maxSize), rc.localScale.z);
        }

        if ((scaleDownY || keyShorter) && rc.localScale.y > minSize)
        {
            rc.localScale = new Vector3(rc.localScale.x, Mathf.Max(rc.localScale.y - scaleSpeed * Time.deltaTime, minSize), rc.localScale.z);
        }

        if ((scaleUpX || keyWider) && Mathf.Abs(rc.localScale.x) < maxSize)
        {
            float sign = Mathf.Sign(rc.localScale.x);
            rc.localScale = new Vector3(
                sign * Mathf.Min(Mathf.Abs(rc.localScale.x) + scaleSpeed * Time.deltaTime, maxSize),
                rc.localScale.y,
                rc.localScale.z
            );
        }

        if ((scaleDownX || keyNarrower) && Mathf.Abs(rc.localScale.x) > minSize)
        {
            float sign = Mathf.Sign(rc.localScale.x);
            rc.localScale = new Vector3(
                sign * Mathf.Max(Mathf.Abs(rc.localScale.x) - scaleSpeed * Time.deltaTime, minSize),
                rc.localScale.y,
                rc.localScale.z
            );
        }

        isTransforming = rotateCW || rotateCCW || scaleUpY || scaleDownY || scaleUpX || scaleDownX ||
            keyCW || keyCCW || keyTaller || keyShorter || keyWider || keyNarrower;
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