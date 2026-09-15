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
                diffZRot = Mathf.Abs(placeZRot - carZRot);
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

                if ((diffZRot <= 7 || (diffZRot >= 353 && diffZRot <= 360)) &&
                    (xSizeDiff <= 0.08f && ySizeDiff <= 0.08f) &&
                    (placeMirrored == carMirrored))
                {
                    Debug.Log("Car placed correctly!");
                    gameObjectsScript.inRightPlace = true;
                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
                        GetComponent<RectTransform>().anchoredPosition;

                    eventData.pointerDrag.GetComponent<RectTransform>().localScale =
                        GetComponent<RectTransform>().localScale;

                    eventData.pointerDrag.GetComponent<RectTransform>().localRotation =
                        GetComponent<RectTransform>().localRotation;

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
}