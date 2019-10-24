using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム選択画面全般のコントローラー
/// </summary>
public class ActionController : MonoBehaviour {
    [SerializeField]
    private GameObject humanClone;
    // 行動回数テキスト
    [SerializeField]
    private Text TActionCount;

    // 行動回数テキスト
    [SerializeField]
    private Text action;

    // 資料の詳細
    [SerializeField]
    private Image ProDImage;

    // 獲得資料配列
    [SerializeField]
    private Image[] getData;

    // 獲得資料アイコン
    [SerializeField]
    private Image[] detailIcon;

    // 任務内容、獲得資料背景、獲得資料詳細
    [SerializeField]
    private Image missionSeat,isData,dataDetail;

    [SerializeField]
    private Sprite[] missionSprite;

    [SerializeField]
    private Image mission;

    // BossUI
    [SerializeField]
    private Image worningChara;

    // boss画像
    [SerializeField]
    private Sprite[] charaSprite; 

    // 人間生成座標
    [SerializeField]
    private GameObject[] createPos = new GameObject[4];

    // ミニゲーム遷移のための数字
    private int[] sceneNum = { 0, 1, 2 };

    // フキダシ付き人間のPrefab配列
    [SerializeField]
    private Image humanPrefab;

    [SerializeField]
    private Image worning;

    // ミニゲーム画像
    [SerializeField]
    private Sprite[] miniGameImage = new Sprite[3];
    // 各UI表示フラグ---------------------------------
    private bool missionOpen = true;  // 任務内容
    private bool dataOpen = true;   // 獲得資料リスト
    private bool datailOpen = true; // 獲得資料詳細
    // -----------------------------------------------
    void DebugTest() {
        foreach (var num in getData) {
            num.GetComponent<Button>().interactable = true;
            num.color = Color.white;
        }
    }

    void Awake() {
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.StopSE();
        SoundManager.Instance.StopVoice();
    }

    void Start () {
        if (Common.Instance.actionCount == 0) {
            SoundManager.Instance.PlaySE(SEName.Warning);
            worningChara.sprite = charaSprite[Common.Instance.gameMode];

            worning.gameObject.SetActive(true);
            StartCoroutine(IsWorning());
            return;
        }

        SoundManager.Instance.PlayBGM(BGMName.Action);

        IsDataSelect();

        action.text = Common.Instance.actionCount.ToString();

        mission.sprite = missionSprite[Common.Instance.gameMode];

        // 各UIを非表示に------------------------
        missionSeat.gameObject.SetActive(false);
        isData.gameObject.SetActive(false);
        dataDetail.gameObject.SetActive(false);
        worning.gameObject.SetActive(false);
        // --------------------------------------

        Common.Instance.Shuffle(createPos);
        Common.Instance.Shuffle(sceneNum);

        CreateHuman();
    }

    /// <summary>
    /// Boss戦
    /// </summary>
    /// <returns></returns>
    public IEnumerator IsWorning()
    {
        yield return new WaitForSeconds(5f);
        Common.Instance.ChangeScene(Common.SceneName.Boss);
    }

    /// <summary>
    /// 獲得資料の設定
    /// </summary>
    public void IsDataSelect() {
        int num = 0;
        foreach(var i in Common.Instance.dataFlag) {
            if (!i) {
                getData[num].GetComponent<Button>().interactable = false;
            }else {
                getData[num].GetComponent<Button>().interactable = true;
                detailIcon[num].color = Color.white;
            }
            num++;
        }
    }

    /// <summary>
    /// 任務確認シート
    /// </summary>
    public void OpenMission()
    {
        missionSeat.gameObject.SetActive(missionOpen);
        if (missionOpen) missionOpen = false;
        else missionOpen = true;
    }

    /// <summary>
    /// 獲得資料シート表示
    /// </summary>
    public void IsDataList()
    {
        isData.gameObject.SetActive(dataOpen);
        if (dataOpen) dataOpen = false;
        else dataOpen = true;
    }

    /// <summary>
    /// 資料詳細表示
    /// </summary>
    public void IsDataDetail(Sprite s)
    {
        if (datailOpen) {
            datailOpen = false;
        } else {
            datailOpen = true;
            ProDImage.sprite = s;
        }
        dataDetail.gameObject.SetActive(datailOpen);
    }

    /// <summary>
    /// 人間生成
    /// </summary>
    public void CreateHuman()
    {
        foreach (var i in sceneNum)
        {
            Image mini = Instantiate(humanPrefab, humanClone.transform) as Image;
            mini.name = mini.name.Replace("(Clone)",i.ToString());
            mini.transform.localPosition = createPos[i].transform.localPosition;

            Image s_mini = mini.transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
            s_mini.sprite = miniGameImage[i];

            HukidashiController hukiCon = mini.GetComponent<HukidashiController>();
            hukiCon.MiniGameNum(i);
        }
    }
}
