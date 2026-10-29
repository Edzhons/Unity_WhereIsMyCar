using System.Collections;
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
    private Image image;
    private Color baseColor = Color.white;
    private bool dragStarted;
    private Coroutine moveRoutine;
    private Coroutine flashRoutine;

    public void Initialize(HanoiGameManager gameManager, int size, Sprite sprite, bool tintBySize)
    {
        manager = gameManager;
        DiskSize = size;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();

        // The sprite shows the disk from slightly above: top face + side.
        // The pivot is the centre of the bottom face, which is where the disk
        // rests on the one below it.
        image.sprite = sprite;
        image.type = Image.Type.Simple;
        image.preserveAspect = false;
        Vector2 spriteSize = sprite.rect.size;
        rectTransform.sizeDelta = spriteSize;
        float topFace = spriteSize.y - manager.diskHeight;
        rectTransform.pivot = new Vector2(0.5f, topFace * 0.5f / spriteSize.y);

        if (tintBySize)
        {
            // Color scale provides a visible distinction without requiring separate sprites.
            float hue = Mathf.InverseLerp(1, HanoiGameManager.MAX_DISKS, size);
            baseColor = Color.HSVToRGB(0.58f - hue * 0.45f, 0.75f, 0.95f);
        }
        else
        {
            baseColor = Color.white;
        }
        image.color = baseColor;

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

    // =========================
    // DRAGGING
    // =========================

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (manager == null || !manager.CanPickUp(this))
        {
            dragStarted = false;
            return;
        }

        dragStarted = true;
        StopMove();
        rectTransform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;
        manager.OnDiskPickedUp();
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

        // A valid rod's OnDrop performs the move before this. Either way the
        // disks fall into place on their rods.
        if (manager != null)
            manager.RefreshAllRodPositions();
    }

    // =========================
    // FALLING INTO PLACE
    // =========================

    /// <summary>Moves over the rod and drops down onto the stack with a bounce.</summary>
    public void MoveTo(Vector2 target, bool animate)
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        StopMove();

        if (!animate || !Application.isPlaying || Vector2.Distance(rectTransform.anchoredPosition, target) < 1f)
        {
            rectTransform.anchoredPosition = target;
            return;
        }

        moveRoutine = StartCoroutine(Fall(target));
    }

    void StopMove()
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }
    }

    IEnumerator Fall(Vector2 target)
    {
        Vector2 start = rectTransform.anchoredPosition;
        bool fromAbove = start.y > target.y;
        float duration = Mathf.Clamp(0.2f + Mathf.Abs(start.y - target.y) / 1500f, 0.25f, 0.55f);
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);

            // Sideways first (over the rod), then down.
            float x = Mathf.Lerp(start.x, target.x, 1f - Mathf.Pow(1f - Mathf.Clamp01(t * 2.5f), 3f));
            float y = Mathf.LerpUnclamped(start.y, target.y, fromAbove ? EaseOutBounce(t) : 1f - (1f - t) * (1f - t));
            rectTransform.anchoredPosition = new Vector2(x, y);
            yield return null;
        }

        rectTransform.anchoredPosition = target;
        moveRoutine = null;
        if (fromAbove && manager != null)
            manager.OnDiskLanded();
    }

    static float EaseOutBounce(float t)
    {
        const float n = 7.5625f;
        const float d = 2.75f;
        if (t < 1f / d) return n * t * t;
        if (t < 2f / d) { t -= 1.5f / d; return n * t * t + 0.75f; }
        if (t < 2.5f / d) { t -= 2.25f / d; return n * t * t + 0.9375f; }
        t -= 2.625f / d;
        return n * t * t + 0.984375f;
    }

    // =========================
    // HINT FLASH
    // =========================

    public void Flash(float duration)
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine(duration));
    }

    public void StopFlash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }
        if (image != null)
            image.color = baseColor;
    }

    IEnumerator FlashRoutine(float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float glow = 0.5f + 0.5f * Mathf.Sin(time * 10f);
            image.color = Color.Lerp(baseColor, new Color(1f, 0.95f, 0.45f), glow * 0.8f);
            yield return null;
        }
        image.color = baseColor;
        flashRoutine = null;
    }
}
