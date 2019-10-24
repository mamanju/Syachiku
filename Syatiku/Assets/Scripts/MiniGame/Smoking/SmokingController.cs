using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmokingController : MonoBehaviour {
    [SerializeField]
    private Image face; // タバコUI、機嫌UI
    [SerializeField]
    private Text[] wordText = new Text[4]; // 選択肢テキスト
    [SerializeField]
    private Text answer; // 問題テキスト
    [SerializeField]
    private Sprite[] faceSprite; // 機嫌画像

    [SerializeField]
    private float time; // 制限時間減少値
    [SerializeField]
    private int answerCount; // 回答権
    [SerializeField]
    private int qLength; // 合計問題数

    [SerializeField]
    private Material smokeMate; // 煙UI

    [SerializeField]
    private CriAtomSource voice; // セリフ音

    private Mushikui mushikui; // Mushikuiコンストラクタ

    private int succesCount,qNum,qCount; // 正解数、今が何番目の問題か

    private float smokeTime; // 煙の数値

    private int textNum;

    private bool isTime = false; // タイマースタートフラグ

    private bool timeOver = false; // タイムオーバーフラグ

    private bool onceFlag = false; // 失敗時のシーン遷移フラグ

    private bool resultFlag = false;

    // パス-----------------------------------------------------------------------
    private string musiFilePath = "CSV/Smoking"; // CSVパス名

    private string talkFilePath = "Text/Smoking/"; // 会話パートテキストパス名

    private string smokePath = "SmokingTalk"; // 喫煙シナリオのPath

    private string badSmokePath = "/SmokingTalkBad"; // 喫煙BadシナリオPath

    private string textPath;
    // ---------------------------------------------------------------------------

    private bool[] badFlags = { false, false, false }; // 不正解フラグ

    private Vector2 tabacoSize; // タバコUIの大きさ

    public GameObject selectUI; // 選択肢UI

    // Use this for initialization
    void Start () {

        SoundManager.Instance.PlayBGM(BGMName.Smoking);

        textNum = Random.Range(0, 4);

        textPath = "Talk" + textNum + "/";
        IsScenario(talkFilePath + textPath + smokePath);
        qNum =  6 * textNum;
        badSmokePath = "Bad" + textNum + badSmokePath;
        //喫煙用ボイスをセット
        SoundManager.Instance.SetVoiceSource(voice);

        selectUI.SetActive(false); // 回答選択UIを非表示
        
        succesCount = 0; // 正解数

        mushikui = new Mushikui(musiFilePath); // 虫食いデータ作成

        Question(); // 問題読み込み
    }

    
    void Update(){

        if (ScenarioController.Instance.IsReachLastInfo())
        {
            smokeTime += 0.1f * Time.deltaTime;
            smokeMate.SetFloat("_FillValue", smokeTime);

            if (resultFlag)
            {
                answerCount = 2;
                resultFlag = false;
                Common.Instance.ChangeScene(Common.SceneName.Result);
            }
            
            if (!badFlags[answerCount])          // 成功時のシナリオ
                StartCoroutine(SelectStart());
            else                                // 失敗時のシナリオ
            {
                if (answerCount == 0 && !onceFlag)
                {
                    onceFlag = true;
                    return;
                }

                if (qLength == 0 && !onceFlag) return;
                answerCount--;
                face.sprite = faceSprite[answerCount];

                IsScenario(talkFilePath + textPath + smokePath + qCount.ToString());
            }

            if (smokeMate.GetFloat("_FillValue") >= 1)
                if (!timeOver) {
                    timeOver = true;
                    Result();
                }
        }
    }

    /// <summary>
    /// 回答選択UIを表示
    /// </summary>
    /// <returns></returns>
    public IEnumerator SelectStart()
    {
        if (!isTime)
        {
            isTime = true;
            selectUI.SetActive(true);
            Question();
        }
        yield return null;
    }

    /// <summary>
    /// 選択肢を選んだ時の処理
    /// </summary>
    /// <param name="text"></param>
    public void ChooseAnswer(Text text) {

        // 回答後の共通の値変化と初期化--------------------
        isTime = false;
        qLength--;
        qCount++;
        smokeTime = 0;
        smokeMate.SetFloat("_FillValue", smokeTime);
        selectUI.SetActive(false);
        //-------------------------------------------------
        
        if (text.text == mushikui.data[qNum].Musikui) {
            Debug.Log("〇");
            succesCount++; // 正解数を加算

            SoundManager.Instance.PlaySE(SEName.CorrectChoice);

            if (qLength <= 0) {
                isTime = true;
                Invoke("SelectFalse", 0.01f);
                Result();
                return;
            }

            qNum++; // 問題Noを加算
            
            IsScenario(talkFilePath + textPath + smokePath + qCount.ToString());

        } else {
            Debug.Log("×");
            SoundManager.Instance.PlaySE(SEName.WrongChoice);

            badFlags[answerCount] = true;
            if (qLength <= 0 || answerCount == 0)
            {
                isTime = true;
                Invoke("SelectFalse", 0.01f);
                Result();
                return;
            }

            qNum++;

            IsScenario(talkFilePath + badSmokePath + answerCount.ToString());
        }
        
        Invoke("SelectFalse", 0.01f);
    }

    /// <summary>
    /// シナリオ呼び出し関数
    /// </summary>
    /// <param name="path"></param>
    public void IsScenario(string path)
    {
        ScenarioController.Instance.BeginScenario(path);
        ScenarioController.Instance.hideButtons();
    }

    /// <summary>
    /// 問題と選択肢をセット
    /// </summary>
    public void Question()
    {
        answer.text = mushikui.data[qNum].Question;
        for(int i = 0; i < mushikui.data[qNum].Select.Length; i++)
        {
            wordText[i].text = mushikui.data[qNum].Select[i];
        }
    }

    /// <summary>
    /// 最終判定
    /// </summary>
    public void Result() {
        selectUI.SetActive(false);
        qLength = 0;
        resultFlag = true;
        if(succesCount >= 4)
        {
            Common.Instance.clearFlag[Common.Instance.miniNum] = true;
            IsScenario(talkFilePath + textPath + "GoodSmokingTalk");
        }
        else
        {
            Common.Instance.clearFlag[Common.Instance.miniNum] = false;
            IsScenario(talkFilePath + badSmokePath + "0");
        }
    }
}
