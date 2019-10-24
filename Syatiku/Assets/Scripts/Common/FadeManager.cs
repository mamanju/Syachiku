using System.Collections;
using UnityEngine;
using DG.Tweening;

public class FadeManager : MonoBehaviour{

    static FadeManager instance;

    public static FadeManager Instance
    {
        get
        {
            if(instance != null)
            {
                return instance;
            }

            instance = FindObjectOfType<FadeManager>();
            return instance;
        }
    }

    void Awake()
    {
        CheckInstance();
    }

    void CheckInstance()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }
        if (instance == this)
        {
            return;
        }

        Destroy(gameObject);
    }

    public void Fade(CanvasGroup canvasGroup, float time, float targetAlpha, TweenCallback callback = null)
    {
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.DOFade(targetAlpha, time).OnComplete(callback);
    }

    public void Fade(UnityEngine.UI.Image image, float time, float targetAlpha, TweenCallback callback = null)
    {
        DOTween.ToAlpha(
            () => image.color,
            color => image.color = color,
            targetAlpha,
            time
        ).OnComplete(callback);
    }

    public void Fade(UnityEngine.UI.Text text, float time, float targetAlpha, TweenCallback callback = null)
    {
        DOTween.ToAlpha(
            () => text.color,
            color => text.color = color,
            targetAlpha,
            time
        ).OnComplete(callback);
    }
}
