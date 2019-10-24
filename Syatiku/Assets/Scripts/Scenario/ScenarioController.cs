using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScenarioController : MonoBehaviour {
    static ScenarioController instance;

    public static ScenarioController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ScenarioController>();
            }

            return instance;
        }
    }

    #region variable

    [SerializeField]
    ScenarioWindow window;
    public List<ScenarioInfo> scenarioInfoList = new List<ScenarioInfo>();

    //参照している情報番号
    public int infoIndex { private get; set; }
    //全ての情報数
    int allInfoNum;
    //表示するセリフ
    System.Text.StringBuilder viewMessage = new System.Text.StringBuilder();
    string allMessage;
    int nextMessageIndex;
    //ログ
    System.Text.StringBuilder logMessage = new System.Text.StringBuilder();
    bool isLogView;

    //文字の表示速度
    [SerializeField]
    float messageViewSpeed = 0.05f;
    float originMessageViewSpeed;
    float messageViewElapsedTime;

    //スキップ中か
    bool isSkip;
    [SerializeField]
    int skipSpeed = 3;

    //オート中か
    bool isAuto;
    [SerializeField, Header("オート時セリフ更新待ち時間")]
    float nextWaitTime = 1f;
    //シナリオ中か
    bool isPlayScenario;
    #endregion

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

    void Awake () {
        CheckInstance();

        window.scenarioCanvas.alpha = 1;

        //DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// シナリオパート開始
    /// </summary>
    /// <param name="path">読み込みたいtxtのパス</param>
    /// <param name="startIndex">最初にシナリオを始めるindex番号</param>
    /// <param name="startVoiceNum">最初にボイスを再生するindex番号</param>
    public void BeginScenario(string path, int startInfoIndex = 0, int startVoiceIndex = 0)
    {
        //必要なデータを取得
        new ImportScenarioInfo(path, ref scenarioInfoList, window, startVoiceIndex);

        Init(startInfoIndex);
        FadeManager.Instance.Fade(window.scenarioCanvas, 1f, 1f, () =>
        {
            SetNextInfo();
            isPlayScenario = true;
        });
    }

    private void Init(int startInfoIndex)
    {
        infoIndex = startInfoIndex;
        allInfoNum = scenarioInfoList.Count;
        originMessageViewSpeed = messageViewSpeed;
        messageViewElapsedTime = 0;
        logMessage = new System.Text.StringBuilder();
        isSkip = false;
        isAuto = false;
        isLogView = false;
        isPlayScenario = false;

        window.Init();
    }

    /// <summary>v
    /// 次のシナリオ情報をセット
    /// </summary>
    /// <param name="num"></param>
    void SetNextInfo()
    {
        window.recommendLight.SetActive(false);
        //感情アイコンの破棄
        for (int i = 0; i < window.emotionsParent.childCount; i++)
        {
            Destroy(window.emotionsParent.GetChild(i).gameObject);
        }
        //セリフウィンドウの初期化
        viewMessage.Length = 0;
        window.message.text = "";
        nextMessageIndex = 0;
        allMessage = scenarioInfoList[infoIndex].message;
        //ボイスストップ
        SoundManager.Instance.StopVoice();
        SoundManager.Instance.StopSE();
        //各コマンド
        if (scenarioInfoList[infoIndex].commandActionList.Count > 0 && scenarioInfoList[infoIndex].fadeTimeList.Count > 0)
        {
            int index = infoIndex;
            float time;
            scenarioInfoList[index].fadeTimeList.TryGetValue(0, out time);
            StartCoroutine(StartCommand(scenarioInfoList[index].commandActionList[0], time, index));
        }
        else
        {
            foreach (var action in scenarioInfoList[infoIndex].commandActionList)
            {
                action();
            }
        }
        infoIndex++;
    }

    IEnumerator StartCommand(System.Action action, float time, int index, int count = 0)
    {
        action();
        yield return new WaitForSeconds(time);
        count++;
        if (count < scenarioInfoList[index].commandActionList.Count)
        {
            System.Action nextAction = scenarioInfoList[index].commandActionList[count];
            scenarioInfoList[index].fadeTimeList.TryGetValue(count, out time);
            yield return StartCommand(nextAction, time, index, count);
        }
    }

    public void hideButtons()
    {
        window.menu.gameObject.SetActive(false);
    }

    void Update()
    {
        //シナリオ中ではない、ログを表示中
        if (!isPlayScenario　|| isLogView)
        {
            if (window.scenarioCanvas.gameObject.activeSelf && window.scenarioCanvas.alpha == 0)
            {
                window.scenarioCanvas.gameObject.SetActive(false);
            }
            return;
        }

        if (!isSkip)
        {
            UpdateMessage();
        }
        else
        {
            UpdateInfoOrMessage(UpdateMessage);
        }
    }

    void UpdateMessage()
    {
        if (!IsShowAllMessage())
        {
            messageViewElapsedTime += Time.deltaTime;

            if (messageViewElapsedTime > messageViewSpeed)
            {
                ShowNextChar();
            }
        }
        else
        {
            ShowRecommendIcon();
            if (IsReachLastInfo()) isPlayScenario = false;
        }
    }

    /// <summary>
    /// タップうながしアイコン表示
    /// </summary>
    void ShowRecommendIcon()
    {
        if (!window.recommendLight.activeSelf)
        {
            window.recommendLight.SetActive(true);
            if (isAuto)
            {
                StartCoroutine(SetNextInfo(nextWaitTime));
            }
        }
    }

    /// <summary>
    /// オート時のセリフ更新メソッド
    /// </summary>
    IEnumerator SetNextInfo(float time)
    {
        yield return new WaitForSeconds(time);
        while (!SoundManager.Instance.IsVoiceEndOrStop())
        {
            yield return null;
        }
        UpdateInfoOrMessage();
    }

    //改行
    char n = '\n';
    /// <summary>
    /// 次の文字を表示
    /// </summary>
    void ShowNextChar()
    {
        if (!isSkip)
        {
            messageViewElapsedTime = 0;
            viewMessage.Append(allMessage[nextMessageIndex]);
            nextMessageIndex++;
        }
        else
        {

            for (int i = 0; i < skipSpeed; i++)
            {
                if (IsShowAllMessage()) break;

                viewMessage.Append(allMessage[nextMessageIndex]);
                nextMessageIndex++;
            }

        }

        window.message.text = viewMessage.ToString();
    }

    /// <summary>
    ///　セリフが全て表示されているか
    /// </summary>
    /// <returns></returns>
    bool IsShowAllMessage()
    {
        return viewMessage.Length == allMessage.Length;
    }

    /// <summary>
    /// セリフをすべて表示
    /// </summary>
    void ShowAllMessage()
    {
        viewMessage.Length = 0;
        viewMessage.Append(allMessage);
        window.message.text = viewMessage.ToString();
    }

    void UpdateInfoOrMessage(System.Action action = null)
    {
        if (IsShowAllMessage())
        {
            if (IsReachLastInfo())
            {
                EndScenario();
            }
            else
            {
                AddLogMessage();
                SetNextInfo();
            }
        }
        else
        {
            if(action != null) action();
        }
    }

    /// <summary>
    /// 最後のシナリオ情報まで到達しているか
    /// </summary>
    public bool IsReachLastInfo()
    {
        return infoIndex == allInfoNum;
    }

    /// <summary>
    /// シナリオ終了
    /// </summary>
    void EndScenario()
    {
        isPlayScenario = false;
        scenarioInfoList.Clear();
    }

    /// <summary>
    /// ログにテキスト追加
    /// </summary>
    void AddLogMessage()
    {
        logMessage.Append("【 " + window.name.text + " 】" + n + allMessage + n);
        window.logText.text = logMessage.ToString();
    }

    #region  OnClickAction

    /// <summary>
    /// 画面を押したとき
    /// </summary>
    public void OnPointerClick()
    {
        if (isPlayScenario)
        {
            //テキスト更新
            if (!isLogView)
            {
                UpdateInfoOrMessage(ShowAllMessage);
                SoundManager.Instance.PlaySE(SEName.Message);
            }
            //ログ非表示
            else
            {
                isLogView = false;
                window.log.SetActive(isLogView);
                window.logButton.color = Color.gray;
            }
        }
    }

    public void OnClickSkipButton()
    {
        if (isPlayScenario)
        {
            isSkip = !isSkip;
            window.skipButton.color = isSkip ? Color.white : Color.gray;
        }
    }

    public void OnClickLogButton()
    {
        isLogView = true;
        window.log.SetActive(isLogView);
        window.logButton.color = Color.white;
        window.scroll.verticalNormalizedPosition = 0;
    }

    public void OnClickAutoButton()
    {
        isAuto = !isAuto;
        window.autoButton.color = isAuto ? Color.white : Color.gray ;
        if (IsShowAllMessage()) StartCoroutine(SetNextInfo(nextWaitTime));
    }

    public void OnClickMenuButton()
    {
        //オープン
        if(window.menuButton.sprite == window.menuSprites[0])
        {

            MoveMenu(window.opneMenuPos);
            window.menuButton.sprite = window.menuSprites[1];
        }
        //クローズ
        else
        {
            MoveMenu(window.closeMenuPos);
            window.menuButton.sprite = window.menuSprites[0];
        }
        SoundManager.Instance.PlaySE(SEName.Menu);
    }

    void MoveMenu(Vector3 targetPos)
    {
        DOTween.To(
            () => window.menu.localPosition,
            position => window.menu.localPosition = position,
            targetPos,
            0.5f
        );
    }

    #endregion
}
