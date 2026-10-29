using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Attach to each disk UI Image. Only the top disk on a rod can be dragged.</summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class HanoiDisk : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int DiskSize { get; private set; }
    public HanoiRod CurrentRod { get; private set; }

    private HanoiGameManager manager;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private RectTransform canvasRect;
    private Camera eventCamera;
    private Vector2 originalAnchoredPosition;
    private Transform originalParent;
    private bool dragStarted;

    public void Initialize(HanoiGameManager gameManager, int size, float width)
    {
        manager = gameManager;
        DiskSize = size;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        rectTransform.sizeDelta = new Vector2(width, manager.diskHeight);
        Image image = GetComponent<Image>();
        if (image != null)
        {
            // Color scale provides a visible distinction without requiring separate sprites.
            float hue = Mathf.InverseLerp(1, manager.diskCount, size);
            image.color = Color.HSVToRGB(0.58f - hue * 0.45f, 0.75f, 0.95f);
        }

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRect = canvas.transform as RectTransform;
            eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        }
    }

    public void SetCurrentRod(HanoiRod rod)
    {
        CurrentRod = rod;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (manager == null || !manager.CanPickUp(this))
        {
            dragStarted = false;
            return;
        }

        dragStarted = true;
        originalParent = transform.parent;
        originalAnchoredPosition = rectTransform.anchoredPosition;
        rectTransform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragStarted || canvasRect == null)
            return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, eventData.position, eventCamera, out Vector2 localPoint))
        {
            rectTransform.position = canvasRect.TransformPoint(localPoint);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragStarted)
            return;

        dragStarted = false;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // A valid rod's OnDrop performs the move. If no valid move occurred,
        // restore the disk to its current rod's stack position.
        if (CurrentRod != null && manager != null)
            manager.RefreshAllRodPositions();
        else
        {
            transform.SetParent(originalParent, true);
            rectTransform.anchoredPosition = originalAnchoredPosition;
        }
    }
}
