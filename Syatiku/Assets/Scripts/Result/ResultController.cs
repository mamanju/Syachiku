using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour {
    
    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private Image enemy,spot;

    [SerializeField]
    private Button backAction;

    [SerializeField]
    private Sprite[] eSprite_white;

    [SerializeField]
    private Sprite[] eSprite_another;

    [SerializeField]
    private GameObject proPrefab;

    [SerializeField]
    private Image[] emotion;

    [SerializeField]
    private float emoTime;

    [SerializeField]
    private RectTransform[] proPos = new RectTransform[2];

    private string[] scoreText = new string[2];

    private bool successFlag = false;
    private bool onceFlag = false;
	// ミニゲームで獲得した情報を表示
	void Start () {
        SoundManager.Instance.StopBGM();
        // Emotionを非表示
        foreach(var i in emotion)
        {
            i.gameObject.SetActive(false);
        }

        // ミニゲーム分岐
        if (Common.Instance.clearFlag[Common.Instance.miniNum]) {
            successFlag = true;
            switch (Common.Instance.miniNum) {
                case 0:
                    scoreText[0] = "書類改ざん";
                    scoreText[1] = "セクハラ";
                    Common.Instance.dataFlag[0] = true;
                    Common.Instance.dataFlag[1] = true;
                    break;
                case 1:
                    scoreText[0] = "スコアノート";
                    scoreText[1] = "ダブル\nブッキング";
                    Common.Instance.dataFlag[2] = true;
                    Common.Instance.dataFlag[3] = true;
                    break;
                case 2:
                    scoreText[0] = "パワハラ";
                    Common.Instance.dataFlag[4] = true;
                    onceFlag = true;
                    break;
            }
        } else {
            successFlag = false;
            scoreText[0] = "スカ";
            scoreText[1] = "スカ";
        }
        StartCoroutine(CreateProperty());
    }

    /// <summary>
    /// 行動選択に戻る
    /// </summary>
    public void ActionBack()
    {
        if (Common.Instance.gameMode == 0)
        {
            if (Common.Instance.actionCount == 0)
                Common.Instance.ChangeScene(Common.SceneName.AnotherBeforeBattle);
            else
                Common.Instance.ChangeScene(Common.SceneName.Action);
        }
        else
        {
            if (Common.Instance.actionCount == 2)
                Common.Instance.ChangeScene(Common.SceneName.Progress);
            else if(Common.Instance.actionCount == 0)
                Common.Instance.ChangeScene(Common.SceneName.BeforeBattle);
            else
                Common.Instance.ChangeScene(Common.SceneName.Action);
        }
           
    }

    /// <summary>
    /// 獲得した情報を表示する
    /// </summary>
    /// <returns></returns>
    public IEnumerator CreateProperty()
    {
        // EnemyとSpotを表示
        yield return new WaitForSeconds(0.5f);
        enemy.gameObject.SetActive(true);
        spot.gameObject.SetActive(true);
        SoundManager.Instance.PlaySE(SEName.Spot2);
        if (Common.Instance.gameMode == 0)
            enemy.sprite = eSprite_another[0];
        else
            enemy.sprite = eSprite_white[0];

        yield return new WaitForSeconds(0.5f);

        // prefabから情報フキダシを生成
        for (int i = 0; i < 2; i++)
        {
            GameObject pro = Instantiate(proPrefab, canvas.transform);
            pro.transform.localPosition = proPos[i].localPosition;
            pro.transform.GetChild(0).GetComponent<Text>().text = scoreText[i];
            SoundManager.Instance.PlaySE(SEName.Hukidashi);
            if (onceFlag) i++;
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1f);
        if (successFlag)
        {
            SoundManager.Instance.PlaySE(SEName.Success);
            if (Common.Instance.gameMode == 0)
            {
                enemy.sprite = eSprite_another[1];
            }
            else
            {
                enemy.sprite = eSprite_white[1];
            }
        }
        else
        {
            SoundManager.Instance.PlaySE(SEName.Failed);
            StartCoroutine(IsEmotion());
        }
        backAction.gameObject.SetActive(true);
    }

    public IEnumerator IsEmotion()
    {
        while (true)
        {
            emotion[0].gameObject.SetActive(true);
            emotion[1].gameObject.SetActive(false);
            yield return new WaitForSeconds(emoTime);
            
            emotion[0].gameObject.SetActive(false);
            emotion[1].gameObject.SetActive(true);
            yield return new WaitForSeconds(emoTime);
        }
    }
}
