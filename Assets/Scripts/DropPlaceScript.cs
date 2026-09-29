using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlaceScript : MonoBehaviour, IDropHandler
{
    private float placeZRot, carZRot, diffZRot;
    private Vector3 placeSize, carSize;
    private float xSizeDiff, ySizeDiff;
    public GameObjectsScript gameObjectsScript;


    void Awake()
    {
        gameObjectsScript = Object.FindFirstObjectByType<GameObjectsScript>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if ((eventData.pointerDrag != null) && Input.GetMouseButtonUp(0) &&
            (!Input.GetMouseButton(2)))
        {
            if (eventData.pointerDrag.tag.Equals(tag))
            {
                placeZRot =
                    eventData.pointerDrag.GetComponent<RectTransform>().transform.eulerAngles.z;
                carZRot = GetComponent<RectTransform>().transform.eulerAngles.z;
                diffZRot = Mathf.Abs(Mathf.DeltaAngle(placeZRot, carZRot));
                Debug.Log("Diff Z Rot: " + diffZRot);

                placeSize = eventData.pointerDrag.GetComponent<RectTransform>().localScale;
                carSize = GetComponent<RectTransform>().localScale;

                // Check if both objects have the same mirror state
                bool placeMirrored = placeSize.x < 0;
                bool carMirrored = carSize.x < 0;

                xSizeDiff = Mathf.Abs(placeSize.x - carSize.x);
                ySizeDiff = Mathf.Abs(placeSize.y - carSize.y);
                Debug.Log("Diff X Size: " + xSizeDiff);
                Debug.Log("Diff Y Size: " + ySizeDiff);

                if (diffZRot <= 15 &&
                    (xSizeDiff <= 0.15f && ySizeDiff <= 0.15f) &&
                    (placeMirrored == carMirrored))
                {
                    Debug.Log("Car placed correctly!");
                    gameObjectsScript.inRightPlace = true;
                    FindFirstObjectByType<GameManagerScript>()
                        .CorrectCarPlaced(); // Juu
                    
                    // Goofy successful placement animation
                    StartCoroutine(CorrectPlaceAnimation(eventData.pointerDrag));

                    switch (eventData.pointerDrag.tag)
                    {
                        case "Garbage":
                            gameObjectsScript.carSoundSource.PlayOneShot(gameObjectsScript.sounds[2]);
                            break;

                        case "Ambulance":
                            gameObjectsScript.carSoundSource.PlayOneShot(gameObjectsScript.sounds[3]);
                            break;

                        case "School":
                            gameObjectsScript.carSoundSource.PlayOneShot(gameObjectsScript.sounds[4]);
                            break;
                        case "Eskavators":
                            gameObjectsScript.carSoundSource.PlayOneShot(gameObjectsScript.sounds[5]);
                            break;
                        case "B2":
                            gameObjectsScript.carSoundSource.PlayOneShot(gameObjectsScript.sounds[6]);
                            break;
                        case "CementaMasina":
                            gameObjectsScript.carSoundSource.PlayOneShot(gameObjectsScript.sounds[7]);
                            break;
                        case "E46":
                            gameObjectsScript.carSoundSource.PlayOneShot(gameObjectsScript.sounds[8]);
                            break;
                        case "E61":
                            gameObjectsScript.carSoundSource.PlayOneShot(gameObjectsScript.sounds[9]);
                            break;
                        case "Policija":
                            gameObjectsScript.carSoundSource.PlayOneShot(gameObjectsScript.sounds[10]);
                            break;
                        case "Traktors":
                            gameObjectsScript.carSoundSource.PlayOneShot(gameObjectsScript.sounds[11]);
                            break;
                        case "Traktors5":
                            gameObjectsScript.carSoundSource.PlayOneShot(gameObjectsScript.sounds[12]);
                            break;
                        case "Ugunsdzeseji":
                            gameObjectsScript.carSoundSource.PlayOneShot(gameObjectsScript.sounds[13]);
                            break;

                        default:
                            Debug.Log("No matching tag found for the dropped object.");
                            break;
                    }
                }

            }
            else
            {
                gameObjectsScript.inRightPlace = false;
                gameObjectsScript.carSoundSource.PlayOneShot(gameObjectsScript.sounds[1]);

                switch (eventData.pointerDrag.tag)
                {
                    case "Garbage":
                        gameObjectsScript.garbageTruck.GetComponent<RectTransform>().localPosition =
                            gameObjectsScript.garbageTruckCoord;
                        break;

                    case "Ambulance":
                        gameObjectsScript.medicine.GetComponent<RectTransform>().localPosition =
                             gameObjectsScript.medicineCoord;
                        break;

                    case "School":
                        gameObjectsScript.schoolBus.GetComponent<RectTransform>().localPosition =
                             gameObjectsScript.schoolBusCoord;
                        break;
                    case "Eskavators":
                        gameObjectsScript.eskavators.GetComponent<RectTransform>().localPosition =
                             gameObjectsScript.eskavatorsCoord;
                        break;
                    case "B2":
                        gameObjectsScript.b2.GetComponent<RectTransform>().localPosition =
                             gameObjectsScript.b2Coord;
                        break;
                    case "CementaMasina":
                        gameObjectsScript.cementaMasina.GetComponent<RectTransform>().localPosition =
                             gameObjectsScript.cementaMasinaCoord;
                        break;
                    case "E46":
                        gameObjectsScript.e46.GetComponent<RectTransform>().localPosition =
                             gameObjectsScript.e46Coord;
                        break;
                    case "E61":
                        gameObjectsScript.e61.GetComponent<RectTransform>().localPosition =
                             gameObjectsScript.e61Coord;
                        break;
                    case "Policija":
                        gameObjectsScript.policija.GetComponent<RectTransform>().localPosition =
                             gameObjectsScript.policijaCoord;
                        break;
                    case "Traktors":
                        gameObjectsScript.traktors.GetComponent<RectTransform>().localPosition =
                             gameObjectsScript.traktorsCoord;
                        break;
                    case "Traktors5":
                        gameObjectsScript.traktors5.GetComponent<RectTransform>().localPosition =
                             gameObjectsScript.traktors5Coord;
                        break;
                    case "Ugunsdzeseji":
                        gameObjectsScript.ugunsdzeseji.GetComponent<RectTransform>().localPosition =
                             gameObjectsScript.ugunsdzesejiCoord;
                        break;

                    default:
                        Debug.Log("No matching tag found for the dropped object.");
                        break;
                }
            }
        }
    }
    private IEnumerator CorrectPlaceAnimation(GameObject car)
    {
        RectTransform carRect = car.GetComponent<RectTransform>();
        RectTransform placeRect = GetComponent<RectTransform>();

        // Final values
        Vector3 finalPosition = placeRect.position;
        Vector3 finalScale = placeRect.localScale;
        float finalRotation = placeRect.eulerAngles.z;

        // Starting values
        Vector3 startPosition = carRect.position;
        Vector3 startScale = carRect.localScale;
        float startRotation = carRect.eulerAngles.z;

        // Find the CENTER of the canvas
        Canvas canvas = carRect.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        Vector3 centerPosition = canvasRect.TransformPoint(canvasRect.rect.center);

        // How huge the car gets
        Vector3 hugeScale = new Vector3(
            startScale.x * 15f,
            startScale.y * 15f,
            startScale.z
        );

        // -------------------------
        // PHASE 1: FLY TO CENTER
        // -------------------------

        float moveToCenterDuration = 0.8f;
        float elapsed = 0f;

        while (elapsed < moveToCenterDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / moveToCenterDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // Move to center
            carRect.position = Vector3.Lerp(
                startPosition,
                centerPosition,
                smoothT
            );

            // Grow MASSIVELY
            carRect.localScale = Vector3.Lerp(
                startScale,
                hugeScale,
                smoothT
            );

            // Spin + flip
            float zRotation = Mathf.Lerp(
                startRotation,
                startRotation + 360f,
                smoothT
            );

            float yRotation = Mathf.Lerp(
                0f,
                720f,
                smoothT
            );

            carRect.rotation = Quaternion.Euler(
                0f,
                yRotation,
                zRotation
            );

            yield return null;
        }

        // Make sure it's exactly centered
        carRect.position = centerPosition;
        carRect.localScale = hugeScale;

        // -------------------------
        // PHASE 2: STAY HUGE
        // -------------------------

        yield return new WaitForSeconds(0.3f);

        // -------------------------
        // PHASE 3: FLY BACK TO PLACE
        // -------------------------

        elapsed = 0f;

        float returnDuration = 1.0f;

        // Current rotation after the crazy part
        Quaternion currentRotation = carRect.rotation;

        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / returnDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // Fly back to parking place
            carRect.position = Vector3.Lerp(
                centerPosition,
                finalPosition,
                smoothT
            );

            // Shrink back down
            carRect.localScale = Vector3.Lerp(
                hugeScale,
                finalScale,
                smoothT
            );

            // Smoothly rotate into correct orientation
            carRect.rotation = Quaternion.Lerp(
                currentRotation,
                Quaternion.Euler(0f, 0f, finalRotation),
                smoothT
            );

            yield return null;
        }

        // -------------------------
        // FINALIZE
        // -------------------------

        carRect.position = finalPosition;
        carRect.localScale = finalScale;
        carRect.rotation = Quaternion.Euler(
            0f,
            0f,
            finalRotation
        );
    }
}