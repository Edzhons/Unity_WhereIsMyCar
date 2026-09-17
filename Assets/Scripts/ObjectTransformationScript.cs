using UnityEngine;

public class ObjectTransformationScript : MonoBehaviour
{
    public GameObjectsScript gameObjectsScript;

    public float minSize = 0.6f;
    public float maxSize = 1.3f;
    public float sizeSpeed = 0.003f;

    void Awake()
    {
        gameObjectsScript = FindFirstObjectByType<GameObjectsScript>();
    }


    void Update()
    {
        if (GameObjectsScript.lastDragged != null)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                GameObjectsScript.lastDragged.GetComponent<RectTransform>().Rotate(
                    0, 0, Time.deltaTime * 12);
            }

            if (Input.GetKey(KeyCode.X))
            {
                GameObjectsScript.lastDragged.GetComponent<RectTransform>().Rotate(
                    0, 0, -Time.deltaTime * 12);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                float y = GameObjectsScript.lastDragged.GetComponent<RectTransform>().localScale.y;

                if (y < maxSize)
                {
                    GameObjectsScript.lastDragged.GetComponent<RectTransform>().localScale =
                        new Vector3(
                            GameObjectsScript.lastDragged.GetComponent<RectTransform>().localScale.x,
                            y + sizeSpeed,
                            1f
                        );
                }
            }


            if (Input.GetKey(KeyCode.DownArrow))
            {
                float y = GameObjectsScript.lastDragged.GetComponent<RectTransform>().localScale.y;

                if (y > minSize)
                {
                    GameObjectsScript.lastDragged.GetComponent<RectTransform>().localScale =
                        new Vector3(
                            GameObjectsScript.lastDragged.GetComponent<RectTransform>().localScale.x,
                            y - sizeSpeed,
                            1f
                        );
                }
            }


            if (Input.GetKey(KeyCode.LeftArrow))
            {
                RectTransform rect =
                    GameObjectsScript.lastDragged.GetComponent<RectTransform>();

                float x = Mathf.Abs(rect.localScale.x);

                if (x > minSize)
                {
                    x -= sizeSpeed;

                    rect.localScale = new Vector3(
                        Mathf.Sign(rect.localScale.x) * x,
                        rect.localScale.y,
                        1f
                    );
                }
            }


            if (Input.GetKey(KeyCode.RightArrow))
            {
                RectTransform rect =
                    GameObjectsScript.lastDragged.GetComponent<RectTransform>();

                float x = Mathf.Abs(rect.localScale.x);

                if (x < maxSize)
                {
                    x += sizeSpeed;

                    rect.localScale = new Vector3(
                        Mathf.Sign(rect.localScale.x) * x,
                        rect.localScale.y,
                        1f
                    );
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                RectTransform rect = GameObjectsScript.lastDragged.GetComponent<RectTransform>();
                rect.localScale = new Vector3(
                    -rect.localScale.x,
                    rect.localScale.y,
                    rect.localScale.z);
            }
        }
    }
}