using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ShockAnimation : MonoBehaviour {

    void Start()
    {
        StartCoroutine(StartAnimation());
        SoundManager.Instance.PlaySE(SEName.Shock);
    }

    IEnumerator StartAnimation()
    {
        RectTransform r = GetComponent<RectTransform>();
        Move(r, new Vector3(r.localPosition.x, r.localPosition.y - 80, 0));
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    protected void Move(RectTransform r, Vector3 targetPos)
    {
        DOTween.To(
        () => r.localPosition,
        p => r.localPosition = p,
        targetPos,
        1f
        );
    }
}
