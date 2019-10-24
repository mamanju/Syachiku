using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class anime : MonoBehaviour
{
    public AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public Vector2 inPos;
    public Vector2 outPos;
    public float duration = 1.0f;

    public void SlideIn()
    {
        StartCoroutine(StartSlidePanel(true));
    }

    public void SlideOut()
    {
        StartCoroutine(StartSlidePanel(false));
    }
    private IEnumerator StartSlidePanel(bool isSlideIn)
    {
        float startTime = Time.time;
        Vector2 startPos = transform.localPosition;
        Vector2 moveDistance;

        if (isSlideIn)
            moveDistance = (inPos - startPos);
        else
        {
            moveDistance = (outPos - startPos);
        }

        while ((Time.time - startTime) < duration)
        {
            transform.localPosition = startPos + moveDistance * animationCurve.Evaluate((Time.time - startTime) / duration);
            yield return 0;
        }
        transform.localPosition = startPos + moveDistance;
    }
}
