using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlickPartController : MonoBehaviour {

    string[,] textContents;

    [SerializeField]
    Sprite[] slappedBossSprites;
    [SerializeField]
    GameObject slappedBoss;
    [SerializeField]
    RectTransform bomRect;
    [SerializeField]
    GameObject bom;
    [SerializeField]
    GameObject flickTextPrefab;
    [SerializeField]
    RectTransform flickTextBox;

    List<FlickTextController> flickTextList = new List<FlickTextController>();

    float spawnTextTimer;
    float spawnTextTime = 1.0f;

    //ボスシーンのゲーム時間
    [SerializeField]
    float gameTime;
    [SerializeField]
    UnityEngine.UI.Text timerText;

    float blinkInterval = 0.2f;
    float blinkTimer = 0;

    [Header("ボスダメージアニメーション")]
    [SerializeField, Header("振動する時間")]
    float duration = 1f;
    [SerializeField, Header("振動する強さ")]
    float strength = 10f;
    [SerializeField, Header("振動する回数")]
    int vibrate = 10;

    int gameMode;
    int clearCount;

    public void Initialize(int gameMode)
    {
        slappedBoss.GetComponent<UnityEngine.UI.Image>().sprite = slappedBossSprites[gameMode];

        textContents = new string[,]
        {
            //Wrong
            { "知らん", "無能が", "役立たず", "黙れ" },
            //Correct
            { "書類改ざん", "セクハラ", "パワハラ", "スコアノート" },
        };
        timerText.text = gameTime.ToString("F0");

        bool[] clearFlag = Common.Instance.clearFlag;
        clearCount = 0;
        //クリアしたミニゲームの数
        for (int i = 0; i < clearFlag.Length; i++)
        {
            clearCount += clearFlag[i] ? 1 : 0;
        }
        gameMode = Common.Instance.gameMode;
    }

    public void UpdateFlickPart () {
        if (spawnTextTimer > spawnTextTime)
        {
            //テキストの生成
            SpawnFlickText();
        }

        UpdateTimer();
    }

    /// <summary>
    /// タイマーの更新
    /// </summary>
    void UpdateTimer()
    {
        //ゲームタイマー
        if (gameTime > 0)
        {
            gameTime -= Time.deltaTime;
            timerText.text = Mathf.Ceil(gameTime).ToString();
        }
        else
        {
            BossScene.Instance.Result();
        }
        //タイマーの点滅
        if(gameTime < 10)
        {
            if (blinkTimer > blinkInterval)
            {
                blinkTimer = 0;
                //少しずつインターバルを短く
                blinkInterval -= 0.002f;
                bom.SetActive(!bom.activeSelf);
            }
            blinkTimer += Time.deltaTime;
        }
        //テキストタイマー
        spawnTextTimer += Time.deltaTime;
    }

    void SpawnFlickText()
    {
        //タイマー初期化
        spawnTextTimer = 0;
        spawnTextTime = Random.Range(0.1f, 2f);

        int randomNum = Random.Range(0, 10);
        //タイプ決定(0:Wrong, 1:Correct)
        int typeNum = GetTypeNum(randomNum);

        //テキストの内容決定
        int textNum = Random.Range(0, textContents.GetLength(1));
        for (int i = 0; i < flickTextList.Count; i++)
        {
            //使われていない（非表示中の）ものがあれば再利用
            if (!flickTextList[i].gameObject.activeSelf)
            {
                flickTextList[i].Initialize(typeNum, textContents[typeNum, textNum]);
                flickTextList[i].gameObject.SetActive(true);
                return;
            }
        }

        //全て使用中なら新しく生成
        FlickTextController text = Instantiate(flickTextPrefab).GetComponent<FlickTextController>();
        //sarcasmMessageBoxの子要素に
        text.transform.SetParent(flickTextBox, false);
        text.Initialize(typeNum, textContents[typeNum, textNum]);
        //リストに追加
        flickTextList.Add(text);
    }

    int GetTypeNum(int randomNum)
    {
        if (gameMode == 1)
        {
            switch (clearCount)
            {
                case 0:
                case 1:
                    return randomNum < 7 ? 0 : 1;
                case 2:
                    return randomNum < 5 ? 0 : 1;
                case 3:
                    return randomNum < 3 ? 0 : 1;
            }
        }
        else
        {
            switch (clearCount)
            {
                case 0:
                    return randomNum < 6 ? 0 : 1;
                case 1:
                    return randomNum < 4 ? 0 : 1;
            }
        }

        return 0;
    }

    public void FlickSuccess()
    {
        BossScene.Instance.ChangeBossState(slappedBoss, duration, true);
        slappedBoss.transform.DOShakePosition(duration, strength, vibrate);
    }
}
