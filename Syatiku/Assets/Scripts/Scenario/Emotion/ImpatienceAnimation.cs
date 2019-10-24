using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ImpatienceAnimation : MonoBehaviour {

    void Start()
    {
        StartCoroutine(StartAnimation());
        SoundManager.Instance.PlaySE(SEName.Impatience);
    }

    IEnumerator StartAnimation()
    {
        RectTransform r = GetComponent<RectTransform>();
        Vector3 pos = r.localPosition;
        Move(r, new Vector3(pos.x + 50, pos.y + 30, 0));
        yield return new WaitForSeconds(0.5f);
        r.localPosition = pos;
        Move(r, new Vector3(pos.x + 80, pos.y + 50, 0));
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    protected void Move(RectTransform r, Vector3 targetPos)
    {
        DOTween.To(
        () => r.localPosition,
        p => r.localPosition = p,
        targetPos,
        0.5f
        );
    }
}
