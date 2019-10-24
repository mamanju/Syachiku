using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WorningController : MonoBehaviour {
    [SerializeField]
    private GameObject[] WLabel = new GameObject[2]; // LebelUI

    [SerializeField]
    private GameObject bossUI;

    [SerializeField]
    private GameObject worning; // worningUI

	// Use this for initialization
	void Start () {

        worning.transform.DOScale(1.5f, 1f).SetLoops(-1);
        Sequence seq = DOTween.Sequence();
        seq.Append(
            worning.transform.DOScale(1.3f, 1f).SetEase(Ease.Linear)
        );
        seq.Append(
            worning.transform.DOScale(1.0f, 1f).SetEase(Ease.Linear)
        );
        seq.SetLoops(-1);

        // WORNING表示
        WLabel[0].transform.DOLocalMoveX(-1850, 2f).SetEase(Ease.Linear).SetLoops(-1);
        WLabel[1].transform.DOLocalMoveX(1850, 2f).SetEase(Ease.Linear).SetLoops(-1);
        bossUI.transform.DOLocalMoveX(0,3f).SetEase(Ease.Linear);
    }
}
