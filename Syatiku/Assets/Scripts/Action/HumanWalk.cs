using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HumanWalk : MonoBehaviour {
    
    [SerializeField]
    private Sprite idle, walk; // 人間画像

    [SerializeField,Header("移動距離限界")]
    private float minDis, maxDis;

    [SerializeField,Header("移動完了までの時間")]
    private float moveTime;

    [SerializeField,Header("回転終了までの時間")]
    private float rotateTime;

    private float count; // 経過時間
    private float beginTime; // 歩行開始までの時間
    private float posX; // 初期位置

    private bool isWalk = false; // 歩行開始フラグ
    
    private int moveDir = 1; // 方向
    private bool dirFlag = false; // 方向転換フラグ
    private Transform humanTransform; // humanUIのTransform

    void Start () {
        beginTime = Random.Range(1, 5);// ランダムに時間を取得
        posX = transform.localPosition.x;
        humanTransform = gameObject.transform.GetChild(0).transform.GetChild(0);
    }
	
	void Update () {
        count += Time.deltaTime; // walk開始の時間まで
        if (count >= beginTime && !isWalk)
            Walk(humanTransform);
	}

    /// <summary>
    /// 歩行関数
    /// </summary>
    /// <returns></returns>
    public void Walk(Transform human)
    {
        isWalk = true; // 歩行開始！

        float distance; // 歩行距離

        distance = Random.Range(minDis, maxDis); // ランダムな値取得

        if (dirFlag)
            distance *= -1; // 右向きならマイナスをかける

        HumanRotate(human, distance);
        
    }

    /// <summary>
    /// 人間歩行モーション
    /// </summary>
    /// <param name="human"></param>
    /// <param name="distance"></param>
    public void HumanRotate(Transform human,float distance)
    {
        Sequence hRotate = DOTween.Sequence();
        hRotate.Append(
            human.DORotate(new Vector3(0, 90, 0), rotateTime).SetEase(Ease.Linear) // 回転
            .OnComplete(() => // 回転終了時
            {
                human.GetComponent<Image>().sprite = walk;  // 画像変更
                human.DORotate(new Vector3(0, 0, 0), rotateTime).SetEase(Ease.Linear); // 元に戻す回転

                transform.DOLocalMoveX(distance, moveTime).SetRelative().SetEase(Ease.Linear) // DOTweenで移動(相対距離)
                .OnComplete(() => // 移動処理終了後
                {
                    human.DORotate(new Vector3(0, 90, 0), rotateTime).SetEase(Ease.Linear) // 回転
                    .OnComplete(() => // 回転終了時
                    {
                        gameObject.transform.GetChild(0).transform.GetChild(0)
                        .GetComponent<Image>().sprite = idle; // 待機画像に変更
                        human.DORotate(new Vector3(0, 0, 0), rotateTime).SetEase(Ease.Linear); // 元に戻す回転
                        // ---------------------------------------------------
                        beginTime = Random.Range(1, 5); // 新たにTimeを設定
                        count = 0; // 開始時間を初期化
                        isWalk = false;
                        HumanDirection(); // 方向を指定
                        // ---------------------------------------------------
                    });
                });
            })
        );
    }

    /// <summary>
    /// 人間の向き設定
    /// </summary>
    public void HumanDirection()
    {
        if (transform.localPosition.x < posX) // 左移動
        {
            if (!dirFlag) return;

            dirFlag = false;
            humanTransform.localScale = new Vector2(1, 1);
            moveDir = -1;
        }
        else if (transform.localPosition.x > posX + 300) // 右移動
        {
            if (dirFlag) return;

            dirFlag = true;
            humanTransform.localScale = new Vector2(-1, 1);
            
            moveDir = 1;
        }
    }
}
