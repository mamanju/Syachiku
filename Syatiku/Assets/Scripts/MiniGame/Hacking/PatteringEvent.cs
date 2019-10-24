using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class PatteringEvent : MonoBehaviour {

    [SerializeField, Tooltip("LowのPaper_1")]
    private RectTransform Low_Paper_1;
    [SerializeField, Tooltip("SpeedyのPaper_1")]
    private RectTransform Speedy_Paper_1;
    [SerializeField, Tooltip("LowのPaper_2")]
    private RectTransform Low_Paper_2;
    [SerializeField, Tooltip("SpeedyのPaper_2")]
    private RectTransform Speedy_Paper_2;
    [SerializeField, Tooltip("LowのPaper_0")]
    private RectTransform Low_Paper_0;
    [SerializeField, Tooltip("SpeedyのPaper_0")]
    private RectTransform Speedy_Paper_0;

    [SerializeField, Tooltip("取得するDocument")]
    private RectTransform getDocument;
    [SerializeField, Tooltip("取得するDocument")]
    private GameObject getDocument_obj;
    [SerializeField, Tooltip("LowAnimationで使うObject")]
    private GameObject LowObject;
    [SerializeField, Tooltip("SpeedyAnimationで使うObject")]
    private GameObject SpeedyObject;
    //[SerializeField]
    //private EventSystem event_system;
    [SerializeField, Tooltip("Get_YellowPeper")]
    private GameObject Get_Yellow;
    [SerializeField, Tooltip("チェックできる所に出すObject")]
    private GameObject GotYellowPaperPrefab;
    [SerializeField, Tooltip("Test Title")]
    private RectTransform Title;
    [SerializeField, Tooltip("PC画面にある付箋")]
    private GameObject Husen_0;
    [SerializeField, Tooltip("PC画面にある付箋")]
    private GameObject Husen_1;

    private HackTap hack_tap;
    private HackMain hack_main;
    private int successCount = 0;

    //いいタイミングかどうか
    private bool _success = false;
    [HideInInspector]
    public bool _PatteringPlay = false;

    [HideInInspector]
    public bool _lowAnimClear = false;
    [HideInInspector]
    public bool _speedyAnimClear = false;
    
    private Sequence quen;
    //一回だけ通したい時に使う
    private bool _onece = false;
    
    // Use this for initialization
    void Start ()
    {
        quen = DOTween.Sequence();
        hack_tap = GetComponent<HackTap>();
        hack_main = GetComponent<HackMain>();
        getDocument_obj.SetActive(false);
        SpeedyObject.SetActive(false);
        LowObject.SetActive(false);
        Get_Yellow.SetActive(false);
        _lowAnimClear = false;
        _speedyAnimClear = false;
        _success = false;
        _PatteringPlay = false;
        successCount = 0;
    }

    private void Update()
    {
        if (_PatteringPlay)
            MemoUI();
        else
            MemoUI();
    }

    /// <summary>
    /// PCに貼ってあるメモがパラパラフェーズの時は見えなくする処理
    /// </summary>
    private void MemoUI()
    {
        if (_PatteringPlay)
        {
            Husen_0.SetActive(false);
            Husen_1.SetActive(false);
        }
        else
        {
            Husen_0.SetActive(true);
            Husen_1.SetActive(true);
        }
    }

    /// <summary>
    /// アニメーション中の色変更
    /// </summary>
    /// <param name="num">0.黄色,1.白</param>
    private void ChangeColor(int num)
    {
        switch (num)
        {
            case 0:
                Low_Paper_1.GetComponent<Image>().color = new Color(255, 255, 0);
                Low_Paper_2.GetComponent<Image>().color = new Color(255, 255, 0);
                Low_Paper_0.GetComponent<Image>().color = new Color(255, 255, 0);
                break;
            case 1:
                Low_Paper_1.GetComponent<Image>().color = new Color(255, 255, 255);
                Low_Paper_2.GetComponent<Image>().color = new Color(255, 255, 255);
                Low_Paper_0.GetComponent<Image>().color = new Color(255, 255, 255);
                break;
            case 2:
                Speedy_Paper_1.GetComponent<Image>().color = new Color(255, 255, 0);
                Speedy_Paper_2.GetComponent<Image>().color = new Color(255, 255, 0);
                Speedy_Paper_0.GetComponent<Image>().color = new Color(255, 255, 0);
                break;
            case 3:
                Speedy_Paper_1.GetComponent<Image>().color = new Color(255, 255, 255);
                Speedy_Paper_2.GetComponent<Image>().color = new Color(255, 255, 255);
                Speedy_Paper_0.GetComponent<Image>().color = new Color(255, 255, 255);
                break;
            default:
                Debug.Log("ColorNum :" + num);
                break;
        }
    }

    /// <summary>
    /// タップした時のテキスト処理
    /// </summary>
    /// <param name="time">テキストが変わる時間</param>
    /// <returns></returns>
    private IEnumerator Wait_Time(float time)
    {
        yield return new WaitForSeconds(time);
        getDocument_obj.SetActive(false);
        _onece = false;
    }

    /// <summary>
    /// Animationをスタートさせる時の処理
    /// </summary>
    /// <param name="time">待ち時間</param>
    /// <returns></returns>
    public IEnumerator Start_AnimWaitTime(bool _lowAnim)
    {
        _PatteringPlay = true;
        successCount = 0;
        if (_lowAnim)
            LowObject.SetActive(true);
        else
            SpeedyObject.SetActive(true);

        Title.GetChild(0).GetComponent<Text>().text = "黄色のページをタップしよう！";
        //hack_main.es.enabled = false;
        Title.transform.localPosition = new Vector2(1400f, 0);
        quen.Append(Title.DOLocalMoveX(0f, 1.0f));
        yield return new WaitForSeconds(1f);

        quen.Append(Title.DOLocalMoveX(-1400f, 1.0f)
            .OnComplete(() => hack_main.es.enabled = true));
        yield return new WaitForSeconds(0.5f);

        if (_lowAnim)
            LowAnim();
        else
            SpeedyAnim();
    }

    /// <summary>
    /// アニメーション中のタップの時処理
    /// </summary>
    public void TapResult()
    {
        if (_success)
        {
            if (!_onece)
            {
                getDocument.transform.position = new Vector3(0, 0, 0);
                successCount++;
                getDocument_obj.SetActive(true);
                _onece = true;
            }
        }
        StartCoroutine(Wait_Time(2f));
    }

    /// <summary>
    /// アニメーション終了時処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator End_Anim()
    {
        Title.transform.localPosition = new Vector2(1400, 0);
        hack_main.es.enabled = false;
        quen.Append(Title.DOLocalMoveX(0f, 1.0f));
        yield return new WaitForSeconds(1.0f);

        quen.Append(Title.DOLocalMoveX(-1450f, 1.0f)
            .OnComplete(() => hack_main.es.enabled = true));
        yield return new WaitForSeconds(0.9f);

        PatteResult();
        hack_main.es.enabled = true;
        LowObject.SetActive(false);
        SpeedyObject.SetActive(false);
        _PatteringPlay = false;
    }

    /// <summary>
    /// パラパラの結果
    /// </summary>
    private void PatteResult()
    {
        hack_tap.PlaceButton(11);
        if (successCount >= 2)
        {
            StartCoroutine(GotYellowPaperAnim());
            GameObject _get_yellow = Instantiate(GotYellowPaperPrefab, hack_tap.GetWord.transform);
            _get_yellow.transform.SetAsLastSibling();
        }
    }

    /// <summary>
    /// 黄色の紙を取得した時のアニメーション
    /// </summary>
    /// <returns></returns>
    private IEnumerator GotYellowPaperAnim()
    {
        hack_main.es.enabled = false;
        Get_Yellow.SetActive(true);
        yield return new WaitForSeconds(0.9f);
        Get_Yellow.SetActive(false);
        hack_main.es.enabled = true;
    }

    /// <summary>
    /// SEName.Pageのループ処理
    /// </summary>
    /// <param name="time">time秒ごとに鳴らす</param>
    /// <param name="_low">true=LowAnim, false=SpeedAnim</param>
    /// <returns></returns>
    private IEnumerator SEWaitTime(float time, bool _low)
    {
        for (int i = 0; i < 100; i++)
        {
            if(_low && _lowAnimClear || !_low && _speedyAnimClear)
            {
                SoundManager.Instance.StopSE();
                break;
            }
            yield return new WaitForSeconds(time);
            SoundManager.Instance.PlaySE(SEName.Page);
        }
    }

    /// <summary>
    /// 遅めのアニメーション処理
    /// </summary>
    private void LowAnim()
    {
        Sequence se = DOTween.Sequence();
        StartCoroutine(SEWaitTime(0.35f,true));
        se.Append(Low_Paper_1.DOLocalRotate(new Vector2(0, Low_Paper_1.localRotation.y + 180), 0.2f).SetDelay(0.5f).SetLoops(97, LoopType.Restart))
           .InsertCallback(3.7f, () => { ChangeColor(0); _success = true; })
           .InsertCallback(4.3f, () => { ChangeColor(1); _success = false; })
           .InsertCallback(7.4f, () => { ChangeColor(0); _success = true; })
           .InsertCallback(8.0f, () => { ChangeColor(1); _success = false; })
           .InsertCallback(14.1f, () => { ChangeColor(0); _success = true; })
           .InsertCallback(14.7f, () => { ChangeColor(1); _success = false; })
           .InsertCallback(18.5f, () => { ChangeColor(0); _success = true; })
           .InsertCallback(19.1f, () => { ChangeColor(1); _success = false; })
           .OnComplete(() => { Title.GetChild(0).GetComponent<Text>().text = "終了"; _lowAnimClear = true; StartCoroutine(End_Anim()); });
    }

    /// <summary>
    /// Animationのイベント処理（Loopバージョン）
    /// </summary>
    private void SpeedyAnim()
    {
        Sequence seq = DOTween.Sequence();
        StartCoroutine(SEWaitTime(0.2f,false));
        seq.Append(Speedy_Paper_1.DOLocalRotate(new Vector2(0, Speedy_Paper_1.localRotation.y + 180), 0.16f).SetDelay(0.1f).SetLoops(120, LoopType.Restart))
           .InsertCallback(3.0f, () => { ChangeColor(2); _success = true; })
           .InsertCallback(3.4f, () => { ChangeColor(3); _success = false; })
           .InsertCallback(8.4f, () => { ChangeColor(2); _success = true; })
           .InsertCallback(8.8f, () => { ChangeColor(3); _success = false; })
           .InsertCallback(10.6f, () => { ChangeColor(2); _success = true; })
           .InsertCallback(11.0f, () => { ChangeColor(3); _success = false; })
           .InsertCallback(16.8f, () => { ChangeColor(2); _success = true; })
           .InsertCallback(17.2f, () => { ChangeColor(3); _success = false; })
           .OnComplete(() => { Title.GetChild(0).GetComponent<Text>().text = "終了"; _speedyAnimClear = true; StartCoroutine(End_Anim()); });
    }
}