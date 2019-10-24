using System.Collections;
using UnityEngine;
using DG.Tweening;

public class AngryAnimation : MonoBehaviour {

    bool isAngry;

    void Start()
    {
        isAngry = gameObject.name.Contains("Angry");
        StartCoroutine(StartAnimation());
    }

    IEnumerator StartAnimation()
    {
        float time = 1f;
        RectTransform r = GetComponent<RectTransform>();
        Vector3 pos = r.localPosition;
        Vector3 targetPos = Vector3.zero;
        if (isAngry)
        {
            targetPos = new Vector3(pos.x + 50, pos.y + 80, 0);
            time = 0.5f;
            SoundManager.Instance.PlaySE(SEName.Angry);
        }
        else
        {
            if (gameObject.name.Contains("Sigh"))
            {
                targetPos = new Vector3(pos.x - 60, pos.y - 60, 0);
                SoundManager.Instance.PlaySE(SEName.Sigh);
            }
            else
            {
                targetPos = new Vector3(pos.x + 40, pos.y + 80, 0);
                time = 2f;
                SoundManager.Instance.PlaySE(SEName.Question);
            }
        }
        UnityEngine.UI.Image i = GetComponent<UnityEngine.UI.Image>();

        Move(r, targetPos, time);
        yield return new WaitForSeconds(time);

        if (isAngry)
        {
            r.localPosition = pos;
            i.color = Color.white;
            Move(r, targetPos, time);
            yield return new WaitForSeconds(time);
        }
        Destroy(gameObject);
    }

    protected void Move(RectTransform r, Vector3 targetPos, float time)
    {
        DOTween.To(
        () => r.localPosition,
        p => r.localPosition = p,
        targetPos,
        time
        );
    }
}
