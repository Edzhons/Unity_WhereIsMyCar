using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class FlyingObjectControllerScript : MonoBehaviour
{
    [HideInInspector]
    public float speed = 1f;
    public float fadeDuration = 1.5f;
    public float waveAmplitude = 25f;
    public float waveFrequency = 1f;
    private GameObjectsScript gameObjectsScript;
    private ScreenBoundaryScript screenBoundaryScript;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool isFadingOut = false;
    private bool isExploading = false;
    private Image image;
    private Color originalColor;



    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rectTransform = GetComponent<RectTransform>();

        image = GetComponent<Image>();
        if (image != null)
            originalColor = image.color;

        gameObjectsScript = Object.FindFirstObjectByType<GameObjectsScript>();
        screenBoundaryScript = Object.FindFirstObjectByType<ScreenBoundaryScript>();
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private void Update()
    {
        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        rectTransform.anchoredPosition +=
           new Vector2(-speed * Time.deltaTime, waveOffset * Time.deltaTime);

        if (speed > 0 && transform.position.x < (screenBoundaryScript.worldBounds.xMin + 80)
            && !isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;
        }

        if (speed < 0 && transform.position.x > (screenBoundaryScript.worldBounds.xMax - 80)
            && !isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;
        }

        // For Android
        Vector2 inputPosition;
        if (!TryGetInputPosition(out inputPosition))
            return;

        if (inputPosition.x < 0 || inputPosition.x > Screen.width ||
            inputPosition.y < 0 || inputPosition.y > Screen.height ||
            Camera.main == null)
            return;



        if (CompareTag("Bomb") && !isExploading &&
            RectTransformUtility.RectangleContainsScreenPoint(
                rectTransform, inputPosition, Camera.main))
        {
            Debug.Log("The cursor collided with a bomb!");
            TriggerExplosion();
        }


        if (GameObjectsScript.isDragging &&
            GameObjectsScript.lastDragged != null &&
            !isFadingOut &&
            RectTransformUtility.RectangleContainsScreenPoint(
                rectTransform, inputPosition, Camera.main))
        {
            Debug.Log("The cursor collided with a flying object!");

            // Save the car before clearing the drag reference.
            GameObject hitCar = GameObjectsScript.lastDragged;

            // Remove one life.
            GameManagerScript gameManager =
                FindFirstObjectByType<GameManagerScript>();

            if (gameManager != null)
            {
                gameManager.LoseLife();
            }
            else
            {
                Debug.LogError("GameManagerScript was not found!");
            }

            // Stop dragging and play the existing car-hit animation.
            GameObjectsScript.lastDragged = null;
            GameObjectsScript.isDragging = false;

            StartCoroutine(ShrinkAndRespawn(hitCar, 0.5f));

            // Keep the existing flying-object effects.
            if (CompareTag("Bomb"))
                StartAndDestroy(Color.red);
            else
                StartAndDestroy(Color.cyan);
        }

    }

    bool TryGetInputPosition(out Vector2 inputPosition)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        inputPosition = Input.mousePosition;
        return true;

#elif UNITY_ANDROID
                if (Input.touchCount > 0)
                {
                    inputPosition = Input.GetTouch(0).position;
                    return true;
                
                } else {
                    inputPosition = Vector2.zero;
                    return false;
                }
#endif
    }


    IEnumerator ShrinkAndRespawn(GameObject obj, float duration)
    {
        if (obj == null)
            yield break;

        Vector3 originalScale = obj.transform.localScale;
        Quaternion originalRotation = obj.transform.rotation;
        float time = 0f;

        // Keep the existing shrink-and-spin animation.
        while (time < duration)
        {
            if (obj == null)
                yield break;

            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            obj.transform.localScale =
                Vector3.Lerp(originalScale, Vector3.zero, t);

            float angle = Mathf.Lerp(0f, 360f, t);
            obj.transform.rotation =
                originalRotation * Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }

        // Restore the car instead of destroying it.
        if (gameObjectsScript == null)
            gameObjectsScript = FindFirstObjectByType<GameObjectsScript>();

        if (gameObjectsScript != null)
        {
            gameObjectsScript.RandomizeSingleCar(obj);

            CanvasGroup carCanvasGroup = obj.GetComponent<CanvasGroup>();

            if (carCanvasGroup != null)
            {
                carCanvasGroup.alpha = 1f;
                carCanvasGroup.blocksRaycasts = true;
                carCanvasGroup.interactable = true;
            }

            obj.SetActive(true);

            Debug.Log("Car respawned: " + obj.name);
        }
        else
        {
            Debug.LogError("Cannot respawn car: GameObjectsScript not found.");
        }
    }


    public void TriggerExplosion()
    {
        isExploading = true;

        if (gameObjectsScript.carSoundSource != null && gameObjectsScript.sounds != null)
        {
            gameObjectsScript.carSoundSource.PlayOneShot(gameObjectsScript.sounds[6], 5f);
        }

        if (TryGetComponent<Animator>(out Animator animator))
        {
            animator.SetBool("explode", true);
        }

        if (image != null)
        {
            image.color = Color.red;
            StartCoroutine(RecoverColor(0.6f));
        }

        StartCoroutine(Vibrate());
        StartCoroutine(WaitBeforeExplode());
    }

    IEnumerator RecoverColor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (image != null)
        {
            image.color = originalColor;
        }
    }

    IEnumerator Vibrate()
    {
#if UNITY_ANDROID
        Handheld.Vibrate();
#endif

        Vector2 originalPos = rectTransform.anchoredPosition;
        float duration = 0.4f;
        float elapsed = 0f;
        float intensity = 5f;

        while (elapsed < duration)
        {
            rectTransform.anchoredPosition = originalPos + Random.insideUnitCircle * intensity;
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = originalPos;
    }

    IEnumerator WaitBeforeExplode()
    {
        float radius = 0f;
        if (TryGetComponent<CircleCollider2D>(out CircleCollider2D circle))
        {
            radius = circle.radius * transform.localScale.x;
        }

        ExplodeAndDestroyNearbyObjects(radius);

        yield return new WaitForSeconds(1f);
        ExplodeAndDestroyNearbyObjects(radius);
        Destroy(gameObject);
    }

    void ExplodeAndDestroyNearbyObjects(float radius)
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D item in hit)
        {
            if (item != null && item.gameObject != gameObject)
            {
                FlyingObjectControllerScript flyingObject =
                    item.GetComponent<FlyingObjectControllerScript>();
                if (flyingObject != null && !flyingObject.isExploading)
                {
                    flyingObject.StartAndDestroy(Color.cyan);
                }
            }
        }
    }

    public void StartAndDestroy(Color color)
    {
        if (!isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;

            if (image != null)
            {
                image.color = color;
                StartCoroutine(RecoverColor(0.5f));
            }

            StartCoroutine(Vibrate());
            if (gameObjectsScript.carSoundSource != null && gameObjectsScript.sounds != null)
            {
                gameObjectsScript.carSoundSource.PlayOneShot(gameObjectsScript.sounds[5]);
            }
        }
    }


    IEnumerator FadeOutAndDestroy()
    {
        float time = 0f;
        float startAlpha = canvasGroup.alpha;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        Destroy(gameObject);
    }
}