using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HackBoss : MonoBehaviour {

    [SerializeField, Tooltip("メーターの上司Object")]
    private GameObject Boss;
    [SerializeField, Tooltip("ドアの上司Object")]
    private GameObject ComeBoss;
    [SerializeField, Tooltip("Zoom object")]
    private GameObject Zoom;
    [SerializeField, Header("ボスの質問に答える時の選択Object")]
    private GameObject ChooseObject;
    [SerializeField, Header("ボスの質問に答える時のText")]
    private Text chose_text;
    [SerializeField, Tooltip("矢印")]
    private RectTransform Yazirusi;

    [HideInInspector]
    public int comingCount = 0;
    private RectTransform boss_rect;

    private HackTap hack_tap;
    private HackMain hack_main;
    private PatteringEvent patte;
    private IntoPCAction into_pc;
    private BossText boss_text;
    [Header("上司が待機してる時間")]
    public float BossTimer = 10.0f;
    private float Bosswait;
    private float req = 0f;
    private bool _commingboss = false;
    private bool _gameover = false;
    [HideInInspector]
    public bool _choosing = false;
    private bool _chooseTap = false;
    //ボスのテキストがmaxまで来た時の判定
    private bool _maxtext = false;
    //ゲージのアニメーション判定
    private bool _onece = false;

    private bool _tapSuccece = false;

    private int rand = 0;
    private int rand_count = 0;
    private int time_state = 0;
    private float time = 0f;
    private float timer = 0f;

    private Sequence sequence;
    private Sequence seq2;

    // Use this for initialization
    void Start () {
        hack_tap = GetComponent<HackTap>();
        hack_main = GetComponent<HackMain>();
        patte = GetComponent<PatteringEvent>();
        into_pc = GetComponent<IntoPCAction>();
        boss_text = GetComponent<BossText>();
        boss_rect = Boss.GetComponent<RectTransform>();

        ChooseObject.SetActive(false);
        ComeBoss.SetActive(false);
        _commingboss = false;
        _gameover = false;
        _choosing = false;
        _chooseTap = false;
        _maxtext = false;
        _onece = false;
        _tapSuccece = false;
        comingCount = 0;
        rand_count = 0;
        time_state = 0;
        Bosswait = BossTimer;
        req = 0f;
        timer = hack_main.timer;
        sequence = DOTween.Sequence();
        seq2 = DOTween.Sequence();
        Boss.transform.localPosition = new Vector2(-885, -277);
        time = 5f;
        sequence.Append(Yazirusi.DOLocalMoveX(875, timer).SetEase(Ease.Linear));
    }
	
	// Update is called once per frame
	void Update () {
        if (hack_main._timerActive && !into_pc._isWindowAnim && !_commingboss && !patte._PatteringPlay && !_tapSuccece)
        {
            req += Time.deltaTime;
            sequence.Play();
            switch (time_state)
            {
                case 0:
                    if (req > 23f)
                    {
                        seq2.Append(sequence.Pause()); MoveBoss(); Debug.Log("一回目");
                        time_state++;
                    }
                    break;
                case 1:
                    if (req > 102f)
                    {
                        seq2.Append(sequence.Pause()); MoveBoss(); Debug.Log("二回目");
                        time_state++;
                    }
                    break;
                case 2:
                    sequence.Append(sequence.OnComplete(() => {
                        Common.Instance.clearFlag[Common.Instance.miniNum] = false;
                        Common.Instance.ChangeScene(Common.SceneName.Result); }));
                    return;
            }
        }
        else
        {
            sequence.Pause();
            _onece = false;
        }
        if (_tapSuccece)
        {
            time -= Time.deltaTime;
            sequence.Pause();

            if (time <= 0.0f)
            {
                //req += 5f;
                _tapSuccece = false;
                sequence.Play();
            }
        }
        //ボスランダム処理
        //if (hack_main._timerActive && !into_pc._isWindowAnim && !_commingboss)
        //{
        //    req -= Time.deltaTime;
        //    if (req <= 0f)
        //    {
        //        rand = Random.Range(0, 4);
        //        if (rand == 1 && !patte._PatteringPlay || rand_count == 3 && !patte._PatteringPlay)
        //        {
        //            if (!_maxtext)
        //            {
        //                boss_rect.transform.DOMoveX(boss_rect.transform.position.x + 2.8f, 0.5f).SetEase(Ease.Linear).OnComplete(() => MoveBoss());
        //                rand_count = 0;
        //            }
        //        }else
        //            rand_count++;

        //        req = 3f;
        //    }
        //}
        //ボスが来た時のタイマー処理
        if (_commingboss)
        {
            Bosswait -= Time.deltaTime;
            chose_text.text = Bosswait.ToString("f1");
            if (_chooseTap)
            {
                //Boss.transform.localPosition = new Vector2(-885, -277);
                ComeBoss.SetActive(false);
                hack_tap.PlaceButton(12);
                _chooseTap = false;
                _commingboss = false;
            }
            else if(Bosswait <= 0.0f)
            {
                //Boss.transform.localPosition = new Vector2(-885, -277);
                ComeBoss.SetActive(false);
                hack_tap.PlaceButton(12);
                _choosing = false;
                _chooseTap = false;
                _commingboss = false;
                Zoom.SetActive(true);
                seq2.Append(sequence.Play());
            }
        }
	}

    /// <summary>
    /// 上司が来て待ってる時の処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator WatchBoss(float time)
    {
        SoundManager.Instance.PlaySE(SEName.EnemyCome);
        yield return new WaitForSeconds(time);
        hack_main.es.enabled = true;
        _commingboss = true;
        ChooseObject.SetActive(true);
    }

    /// <summary>
    /// ゲージの矢印アニメーション
    /// </summary>
    private void BossAnim()
    {
        seq2 = DOTween.Sequence();
        seq2.InsertCallback(22f, () => { sequence.Append(sequence.Pause()); MoveBoss(); Debug.Log("一回目"); })
                .InsertCallback(102f, () => { sequence.Append(sequence.Pause()); MoveBoss(); Debug.Log("二回目"); });
        sequence.Play();
        seq2.Play();
    }

    /// <summary>
    /// メーターの上司が動く処理
    /// </summary>
    public void MoveBoss()
    {
        comingCount++;
        if (comingCount == 8)
            _maxtext = true;

        _onece = false;
        Zoom.SetActive(false);
        hack_main.es.enabled = false;
        ComeOnBoss();
        //if (comingCount%4 == 0)
        //{
        //    Zoom.SetActive(false);
        //    hack_main.es.enabled = false;
        //    ComeOnBoss();
        //}
    }

    /// <summary>
    /// 上司が部屋に来た時の処理
    /// </summary>
    public void ComeOnBoss()
    {
        Bosswait = BossTimer;
        chose_text.text = Bosswait.ToString("f1");
        _choosing = true;
        boss_text.AddText();
        hack_tap.PlaceButton(13);
        hack_tap.PlaceButton(11);

        if (!ComeBoss.activeSelf)
        {
            ComeBoss.SetActive(true);
            StartCoroutine(WatchBoss(1.5f));
        }
    }

    /// <summary>
    /// 選択ボタン処理
    /// </summary>
    public void ChooseButton(Text tx)
    {
        Zoom.SetActive(true);
        Bosswait = BossTimer;
        seq2.Append(sequence.Play());
        _choosing = false;
        _chooseTap = true;
        ChooseCheck(tx);
    }

    /// <summary>
    /// 選択ボタンで正解をおしたかどうか
    /// </summary>
    private void ChooseCheck(Text btn)
    {
        string boss_str = boss_text.stren[boss_text.stren.Length-1];
        if (btn.text == boss_str)
        {
            Debug.Log("正解！");
            SoundManager.Instance.PlaySE(SEName.TapAction);
            _tapSuccece = true;
            time += 5f;
        }
        else
        {
            Debug.Log("不正解");
            SoundManager.Instance.PlaySE(SEName.PasswordMiss);
            _tapSuccece = false;
        }
    }
}