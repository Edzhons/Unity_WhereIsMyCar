using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropScript : MonoBehaviour,
    IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObjectsScript gameObjectsScript;
    public ScreenBoundaryScript screenBoundaryScript;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Camera mainCamera;
    private Vector3 dragOffsetWorld;

    // Used by the flying-object collision script to prevent an immediate re-drag.
    public static bool blockDraggingUntilMouseUp = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (gameObjectsScript == null)
            gameObjectsScript = FindFirstObjectByType<GameObjectsScript>();

        if (screenBoundaryScript == null)
            screenBoundaryScript = FindFirstObjectByType<ScreenBoundaryScript>();

        Canvas canvas = GetComponentInParent<Canvas>();
        mainCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera
            : Camera.main;

        if (rectTransform == null)
            Debug.LogError("DragAndDropScript needs a RectTransform on " + gameObject.name);
        if (screenBoundaryScript == null)
            Debug.LogWarning("ScreenBoundaryScript was not found in the scene.");
        if (gameObjectsScript == null)
            Debug.LogWarning("GameObjectsScript was not found in the scene.");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (gameObjectsScript != null &&
            gameObjectsScript.carSoundSource != null &&
            gameObjectsScript.sounds != null &&
            gameObjectsScript.sounds.Length > 0 &&
            gameObjectsScript.sounds[0] != null)
        {
            gameObjectsScript.carSoundSource.PlayOneShot(gameObjectsScript.sounds[0]);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (blockDraggingUntilMouseUp || rectTransform == null)
            return;

        if (!TryScreenToWorldPoint(eventData.position, out Vector3 pointerWorldPosition))
            return;

        GameObjectsScript.isDragging = true;
        GameObjectsScript.lastDragged = eventData.pointerDrag;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        if (transform.parent != null)
        {
            int lastIndex = transform.parent.childCount - 1;
            transform.SetSiblingIndex(Mathf.Max(0, lastIndex - 1));
        }

        dragOffsetWorld = transform.position - pointerWorldPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (blockDraggingUntilMouseUp || rectTransform == null)
            return;

        // PointerEventData supports both mouse and touchscreen dragging.
        if (!TryScreenToWorldPoint(eventData.position, out Vector3 pointerWorldPosition))
            return;

        Vector3 targetPosition = pointerWorldPosition + dragOffsetWorld;
        if (screenBoundaryScript != null)
            targetPosition = screenBoundaryScript.GetClampedPosition(targetPosition);

        rectTransform.position = targetPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObjectsScript.isDragging = false;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        if (gameObjectsScript != null)
        {
            if (gameObjectsScript.inRightPlace)
            {
                if (canvasGroup != null)
                    canvasGroup.blocksRaycasts = false;
                GameObjectsScript.lastDragged = null;
            }

            gameObjectsScript.inRightPlace = false;
        }
    }

    public void CancelDrag()
    {
        GameObjectsScript.isDragging = false;
        GameObjectsScript.lastDragged = null;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }

    void Update()
    {
        if (!blockDraggingUntilMouseUp)
            return;

#if UNITY_ANDROID
        if (Input.touchCount == 0)
        {
            blockDraggingUntilMouseUp = false;
            Debug.Log("Touch released - dragging enabled.");
        }
#else
    if (Input.GetMouseButtonUp(0))
    {
        blockDraggingUntilMouseUp = false;
        Debug.Log("Mouse released - dragging enabled.");
    }
#endif
    }

    private bool TryScreenToWorldPoint(Vector2 screenPosition, out Vector3 worldPosition)
    {
        worldPosition = Vector3.zero;
        if (mainCamera == null)
            mainCamera = Camera.main;
        if (mainCamera == null)
            return false;

        float depth = screenBoundaryScript != null
            ? screenBoundaryScript.screenPoint.z
            : Mathf.Abs(mainCamera.transform.position.z - transform.position.z);

        worldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(screenPosition.x, screenPosition.y, depth));
        return true;
    }
}
