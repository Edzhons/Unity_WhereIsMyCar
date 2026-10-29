using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// TEST CHEAT for phones: tap the rubber duck on the dashboard 5 times quickly
/// to get 50 stars. Every tap makes the duck wiggle.
/// </summary>
public class DuckSecret : MonoBehaviour, IPointerClickHandler
{
    public int tapsNeeded = 5;
    public float maxTimeBetweenTaps = 0.7f;
    public RadioStarDisplay radio;

    public AudioSource audioSource;
    public AudioClip tapSound;
    public AudioClip rewardSound;

    private int taps;
    private float lastTapTime = -10f;
    private Coroutine wiggleRoutine;

    public void OnPointerClick(PointerEventData eventData)
    {
        float now = Time.unscaledTime;
        taps = now - lastTapTime <= maxTimeBetweenTaps ? taps + 1 : 1;
        lastTapTime = now;

        if (wiggleRoutine != null)
            StopCoroutine(wiggleRoutine);
        wiggleRoutine = StartCoroutine(Wiggle());

        if (taps < tapsNeeded)
        {
            Play(tapSound);
            return;
        }

        taps = 0;
        StarBank.Deposit(StarBank.CHEAT_STARS);
        Play(rewardSound);
        if (radio != null)
            radio.ShowMessage("+" + StarBank.CHEAT_STARS);
        Debug.Log("TEST: duck gave " + StarBank.CHEAT_STARS + " stars");
    }

    IEnumerator Wiggle()
    {
        float time = 0f;
        while (time < 0.4f)
        {
            time += Time.unscaledDeltaTime;
            float angle = Mathf.Sin(time * 40f) * 12f * (1f - time / 0.4f);
            transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }
        transform.localRotation = Quaternion.identity;
        wiggleRoutine = null;
    }

    void Play(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
