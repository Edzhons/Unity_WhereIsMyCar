using UnityEngine;

public class GameObjectsScript : MonoBehaviour
{
    public GameObject garbageTruck;
    public GameObject medicine;
    public GameObject schoolBus;
    public GameObject eskavators;
    public GameObject b2;
    public GameObject cementaMasina;
    public GameObject e46;
    public GameObject e61;
    public GameObject policija;
    public GameObject traktors;
    public GameObject traktors5;
    public GameObject ugunsdzeseji;


    // =========================
    // RANDOM SPAWN ARRAYS
    // =========================

    public GameObject[] cars;
    public GameObject[] carPlaces;

    public Transform[] carSpawnPoints;
    public Transform[] carPlaceSpawnPoints;


    [HideInInspector]
    public Vector2 garbageTruckCoord;

    [HideInInspector]
    public Vector2 medicineCoord;

    [HideInInspector]
    public Vector2 schoolBusCoord;

    public Vector2 eskavatorsCoord;
    public Vector2 b2Coord;
    public Vector2 cementaMasinaCoord;
    public Vector2 e46Coord;
    public Vector2 e61Coord;
    public Vector2 policijaCoord;
    public Vector2 traktorsCoord;
    public Vector2 traktors5Coord;
    public Vector2 ugunsdzesejiCoord;


    public Canvas canvas;
    public AudioSource carSoundSource;
    public AudioClip[] sounds;

    [HideInInspector]
    public bool inRightPlace = false;

    public static GameObject lastDragged = null;
    public static bool isDragging = false;


    void Awake()
    {
        RandomizeObjects();

        garbageTruckCoord = garbageTruck.GetComponent<RectTransform>().anchoredPosition;
        medicineCoord = medicine.GetComponent<RectTransform>().anchoredPosition;
        schoolBusCoord = schoolBus.GetComponent<RectTransform>().anchoredPosition;
        eskavatorsCoord = eskavators.GetComponent<RectTransform>().anchoredPosition;
        b2Coord = b2.GetComponent<RectTransform>().anchoredPosition;
        cementaMasinaCoord = cementaMasina.GetComponent<RectTransform>().anchoredPosition;
        e46Coord = e46.GetComponent<RectTransform>().anchoredPosition;
        e61Coord = e61.GetComponent<RectTransform>().anchoredPosition;
        policijaCoord = policija.GetComponent<RectTransform>().anchoredPosition;
        traktorsCoord = traktors.GetComponent<RectTransform>().anchoredPosition;
        traktors5Coord = traktors5.GetComponent<RectTransform>().anchoredPosition;
        ugunsdzesejiCoord = ugunsdzeseji.GetComponent<RectTransform>().anchoredPosition;
    }


    void RandomizeObjects()
    {
        // Shuffle car spawn points
        Shuffle(carSpawnPoints);

        // Shuffle car place spawn points
        Shuffle(carPlaceSpawnPoints);


        // Put each car into a random spawn point
        for (int i = 0; i < cars.Length; i++)
        {
            RectTransform carRect =
                cars[i].GetComponent<RectTransform>();

            // Random position
            carRect.anchoredPosition =
                carSpawnPoints[i].GetComponent<RectTransform>().anchoredPosition;

            // Random rotation
            float randomRotation = Random.Range(-45f, 45f);
            carRect.localRotation =
                Quaternion.Euler(0f, 0f, randomRotation);

            // Random size
            float randomX = Random.Range(0.6f, 1.3f);
            float randomY = Random.Range(0.6f, 1.3f);

            carRect.localScale = new Vector3(
                randomX,
                randomY,
                carRect.localScale.z
            );

            // Random mirror
            bool randomMirror = Random.Range(0, 2) == 1;

            if (randomMirror)
            {
                carRect.localScale = new Vector3(
                    -carRect.localScale.x,
                    carRect.localScale.y,
                    carRect.localScale.z
                );
            }
        }


        // Put each car place into a random spawn point
        for (int i = 0; i < carPlaces.Length; i++)
        {
            RectTransform placeRect =
                carPlaces[i].GetComponent<RectTransform>();

            // Random position
            placeRect.anchoredPosition =
                carPlaceSpawnPoints[i].GetComponent<RectTransform>().anchoredPosition;

            // Random rotation
            float randomRotation = Random.Range(-45f, 45f);
            placeRect.localRotation =
                Quaternion.Euler(0f, 0f, randomRotation);

            // Random size
            float randomX = Random.Range(0.6f, 1.3f);
            float randomY = Random.Range(0.6f, 1.3f);

            placeRect.localScale = new Vector3(
                randomX,
                randomY,
                placeRect.localScale.z
            );

            // Random mirror
            bool randomMirror = Random.Range(0, 2) == 1;

            if (randomMirror)
            {
                placeRect.localScale = new Vector3(
                    -placeRect.localScale.x,
                    placeRect.localScale.y,
                    placeRect.localScale.z
                );
            }
        }
    }

    public void RandomizeSingleCar(GameObject car)
    {
        RectTransform carRect = car.GetComponent<RectTransform>();


        // =========================
        // FIND AN EMPTY SPAWN POINT
        // =========================

        int randomIndex = Random.Range(0, carSpawnPoints.Length);

        bool positionOccupied = true;

        while (positionOccupied)
        {
            randomIndex = Random.Range(0, carSpawnPoints.Length);

            positionOccupied = false;

            foreach (GameObject otherCar in cars)
            {
                if (otherCar == car)
                    continue;

                RectTransform otherRect =
                    otherCar.GetComponent<RectTransform>();

                float distance =
                    Vector2.Distance(
                        otherRect.anchoredPosition,
                        carSpawnPoints[randomIndex]
                            .GetComponent<RectTransform>()
                            .anchoredPosition
                    );

                if (distance < 1f)
                {
                    positionOccupied = true;
                    break;
                }
            }
        }


        // =========================
        // APPLY RANDOM POSITION
        // =========================

        carRect.anchoredPosition =
            carSpawnPoints[randomIndex]
            .GetComponent<RectTransform>()
            .anchoredPosition;


        // =========================
        // RANDOM ROTATION
        // =========================

        float randomRotation =
            Random.Range(-45f, 45f);

        carRect.localRotation =
            Quaternion.Euler(0f, 0f, randomRotation);


        // =========================
        // RANDOM SIZE
        // =========================

        float randomX =
            Random.Range(0.6f, 1.3f);

        float randomY =
            Random.Range(0.6f, 1.3f);

        carRect.localScale =
            new Vector3(
                randomX,
                randomY,
                carRect.localScale.z
            );


        // =========================
        // RANDOM MIRROR
        // =========================

        bool randomMirror =
            Random.Range(0, 2) == 1;

        if (randomMirror)
        {
            carRect.localScale =
                new Vector3(
                    -carRect.localScale.x,
                    carRect.localScale.y,
                    carRect.localScale.z
                );
        }


        Debug.Log(
            "Car randomized again: " +
            car.name
        );
    }

    void Shuffle<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}