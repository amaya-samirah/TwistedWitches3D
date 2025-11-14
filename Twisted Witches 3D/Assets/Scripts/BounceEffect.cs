using System.Collections;
using UnityEngine;

public class BounceEffect : MonoBehaviour
{
    public float bounceHeight = 0.3f;
    public float bounceDuration = 0.3f;
    public int bounceCount = 2;

    public void StartBounce()
    {
        StartCoroutine(BounceHandler());
    }

    private IEnumerator BounceHandler()
    {
        Vector3 startPos = transform.position;

        float localHeight = bounceHeight;
        float localDuration = bounceDuration;

        for (int i = 0; i < bounceCount; i++)
        {
            // Call coroutine
            yield return Bounce(startPos, localHeight, localDuration / 2);
            localHeight *= 0.5f;
            localDuration *= 0.8f;
        }

        transform.position = startPos;
    }

    private IEnumerator Bounce(Vector3 start, float height, float duration)
    {
        Vector3 peakHeight = start + Vector3.up * height;
        float elapsedTime = 0f;

        // Move up
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(start, peakHeight, elapsedTime / duration); // Lerp() interloops between 2 points
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0;

        // Move down
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(peakHeight, start, elapsedTime / duration); // Lerp() interloops between 2 points
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
