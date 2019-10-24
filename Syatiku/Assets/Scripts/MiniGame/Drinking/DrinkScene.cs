using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrinkScene : MonoBehaviour {

    // スクリプトのインスタンスの取得
    ButtonController button;
    Denmoku denmoku;
    DenmokuMeter meter;

    // 表示された注文商品が格納されるオブジェクト
    public GameObject menuObject;

    // 商品のPrefabを格納する配列
    public GameObject[] MenuList;

    // 注文数を表示する
    [SerializeField]
    Text[] OrderCounterList;

    // フキダシ-----------------------
    [SerializeField]
    private Sprite[] timeOut;

    [SerializeField]
    private Sprite[] successSprite;

    [SerializeField]
    private Image[] hukidashiImage;
    // ------------------------------

    // 吹き出しを表示する
    [SerializeField]
    GameObject[] HukidashiList;

    // 注文の正誤結果を表示する
    [SerializeField]
    Text[] AnswerList;

    [SerializeField]
    Text TapText;

    // ゲームの残り回数を表示
    [SerializeField]
    Text LimitText;
    
    // 商品IDを保存する配列
    private int[] foodsBox = new int[14];

    // オーダーが入った商品のIDを保存する配列
    private int[] OrderBox = new int[4];

    // オーダーが入った商品の個数を保存する配列
    private int[] OrderCounter = new int[4];

    // オーダーが入った商品画像を表示する場所の配列
    private float[] OrderPos = new float[] {-6.35f, -2.65f, 1.35f, 5.0f};

    private int[] Num = new int[4];

    private int NumCounter = 0;

    private bool NextGameFlg;

    [SerializeField, Range(0, 2), Tooltip("注文の表示時間(秒)")]
    private float Timer;

    [SerializeField, Range(2, 10), Tooltip("回数制限")]
    private int Limit;

    private float OriginPos1;
    private float OriginPos2;
    private float OriginPos3;

    private int ClearQuota;
    private int ClearCount;
    private int ClearScore;

    // デバッグ用のカンニング
    //[SerializeField]
    private string[] AnswerCheck = new string[4];

    [SerializeField, Tooltip("クリア条件を緩くする")]
    private bool ChangeGameMode;
    private bool GameMode;

    //注文の配列の用意
    public void OrderShuffle()
    {
        //商品を格納する配列を用意&シャッフル
        for(int i = 0; i < this.foodsBox.Length; i++)
        {
            this.foodsBox[i] = i;
        }
        Common.Instance.Shuffle(this.foodsBox);
        
        //注文配列・個数配列
        for(int i = 0; i < this.OrderBox.Length; i++)
        {
            this.OrderBox[i] = this.foodsBox[i];
            this.OrderCounter[i] = UnityEngine.Random.Range(1, 5);
        }
    }
    
    //表示する位置をシャッフル
    public void PosShuffle()
    {
        Common.Instance.Shuffle(this.OrderPos);

        //Num配列の用意
        for (int i = 0; i < this.OrderPos.Length; i++)
        {
            if(this.OrderPos[i] == this.OriginPos1)
            {
                this.Num[i] = 0;
            }
            else if(this.OrderPos[i] == this.OriginPos2)
            {
                this.Num[i] = 1;
            }
            else if(this.OrderPos[i] == this.OriginPos3)
            {
                this.Num[i] = 2;
            }
            else
            {
                this.Num[i] = 3;
            }
        }
    }

    // 注文商品を1個ずつランダムな位置に表示して消すを繰り返す
    public IEnumerator OrderMethod()
    {
        for (int i = 0; i < this.OrderBox.Length; i++)
        {
            yield return new WaitForSeconds(1.0f);
            
            // 吹き出しと注文数の表示
            this.OrderHukidashi();

            // 注文商品の表示
            var Menu_Order = Instantiate(this.MenuList[this.OrderBox[i]], new Vector2(OrderPos[i], 3.35f), Quaternion.identity);
            Menu_Order.transform.localScale = new Vector2(0.345f, 0.345f);
            Menu_Order.transform.parent = this.menuObject.transform;

            // 表示された吹き出しと商品を消す
            yield return new WaitForSeconds(this.Timer);
            this.Delete();
            this.OrderCounterOFF();
            this.HukidashiOFF();
        }
        yield return new WaitForSeconds(0.5f);
        button.DrinkSceneButton(true);
        this.NumCounter = 0;
    }

    public void Delete()
    {
        var Delete = this.menuObject.transform;
        for (int i = 0; i < Delete.childCount; i++)
        {
            Destroy(Delete.GetChild(i).gameObject);
        }
    }

    //注文の答えの表示
    public IEnumerator Answer()
    {
        // 残りゲーム回数を減算
        this.Limit--;

        // 注文入力が時間切れの場合
        if (meter.TimeOverFlg)
        {
            for(int i = 0; i < this.AnswerList.Length; i++)
            {
                Vector2 pos = this.AnswerList[i].transform.localPosition;
                pos.y = 30;
                this.AnswerList[i].transform.localPosition = pos;
                this.AnswerList[i].transform.localScale = new Vector2(1.0f, 1.0f);
                this.AnswerList[i].color = new Color(50f / 255f, 50f / 255f, 50f / 255f, 255f / 255f);
                this.AnswerList[i].gameObject.SetActive(true);
            }

            for(int i = 0; i < hukidashiImage.Length; i++)
            {
                hukidashiImage[i].sprite = timeOut[i];
            }

            this.AnswerList[0].text = "時";
            this.AnswerList[1].text = "間";
            this.AnswerList[2].text = "切";
            this.AnswerList[3].text = "れ";
        }
        
        // 時間切れになる前に注文入力が終了した場合
        else
        {
            int ArrayPos = 0;
            for(int i = 0; i < this.OrderBox.Length; i++)
            {
                ArrayPos = Array.IndexOf(denmoku.InputOrderBox, this.OrderBox[i]);
                if (ArrayPos >= 0)
                {
                    if(this.OrderCounter[i] == denmoku.InputOrderCounter[ArrayPos])
                    {
                        this.OutputAnswer(Num[i], true);
                    }
                    else
                    {
                        this.OutputAnswer(Num[i], false);
                    }
                }
                else
                {
                    this.OutputAnswer(Num[i], false);
                }
            }
            if (this.GameMode)
            {
                this.ClearScore += this.ClearCount;
            }
            else
            {
                if (this.ClearCount == 4)
                {
                    this.ClearScore++;
                }
            }
        }
        
        // 吹き出しを表示
        for(int i = 0; i < this.HukidashiList.Length; i++)
        {
            yield return new WaitForSeconds(0.5f);
            this.HukidashiList[i].gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(1.0f);
        this.TapText.gameObject.SetActive(true);
        this.NextGameFlg = true;
    }
    
    //注文を表示する際の、吹き出しと個数を表示させるメソッド
    public void OrderHukidashi()
    {
        this.HukidashiList[this.Num[this.NumCounter]].gameObject.SetActive(true);
        this.OrderCounterList[this.Num[this.NumCounter]].gameObject.SetActive(true);
        this.OrderCounterList[this.Num[this.NumCounter]].text = "× " + this.OrderCounter[this.NumCounter].ToString();
        this.NumCounter++;
    }

    public void OrderCounterOFF()
    {
        for(int i = 0; i < this.OrderCounterList.Length; i++)
        {
            this.OrderCounterList[i].gameObject.SetActive(false);
        }
    }

    public void AnswerResultOFF()
    {
        for(int i = 0; i < AnswerList.Length; i++)
        {
            this.AnswerList[i].gameObject.SetActive(false);
        }
    }
    
    public void HukidashiOFF()
    {
        for(int i = 0; i < this.HukidashiList.Length; i++)
        {
            this.HukidashiList[i].gameObject.SetActive(false);
        }
    }
    
    //注文の正誤判定の表示を管理するメソッド
    public void OutputAnswer(int i, bool b)
    {
        Vector2 pos = this.AnswerList[i].transform.localPosition;
        this.AnswerList[i].gameObject.SetActive(true);
        this.AnswerList[i].transform.localScale = new Vector2(2.0f, 2.0f);
        if (b)
        {
            pos.y = 75;
            this.AnswerList[i].transform.localPosition = pos;
            this.AnswerList[i].text = "○";
            this.AnswerList[i].color = new Color(255f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            this.ClearCount++;
            hukidashiImage[i].sprite = successSprite[0];
            hukidashiImage[i].gameObject.SetActive(true);
            AnswerList[i].gameObject.SetActive(false);
        }
        else
        {
            pos.y = 30;
            this.AnswerList[i].transform.localPosition = pos;
            this.AnswerList[i].text = "×";
            this.AnswerList[i].color = new Color(40f / 255f, 0f / 255f, 255f / 255f, 255f / 255f);
            hukidashiImage[i].sprite = successSprite[1];
            hukidashiImage[i].gameObject.SetActive(true);
            AnswerList[i].gameObject.SetActive(false);
        }
    }

    public void GameStart()
    {
        this.OrderShuffle();
        this.PosShuffle();
        StartCoroutine(this.OrderMethod());
        for (int i = 0; i < denmoku.InputOrderBox.Length; i++)
        {
            denmoku.InputOrderBox[i] = -1;
            denmoku.InputOrderCounter[i] = 0;
        }
        this.TapText.text = "画面をタップ！";
        meter.TimeOverFlg = false;
        this.ClearCount = 0;

        for (int i = 0; i < hukidashiImage.Length; i++)
        {
            //AnswerList[i].gameObject.SetActive(false);
            hukidashiImage[i].gameObject.SetActive(false);
        }

        // デバッグ用の処理
        //this.LookAnswer();
    }
   
    void Start () {
        for(int i = 0; i < hukidashiImage.Length; i++)
        {
            AnswerList[i].gameObject.SetActive(false);
            hukidashiImage[i].gameObject.SetActive(false);
        }

        //ゲームの初期状態を用意する処理
        if (this.ChangeGameMode)
        {
            this.GameMode = true;
        }
        else
        {
            this.GameMode = false;
        }
        button = GetComponent<ButtonController>();
        denmoku = GetComponent<Denmoku>();
        meter = GetComponent<DenmokuMeter>();
        this.OriginPos1 = this.OrderPos[0];
        this.OriginPos2 = this.OrderPos[1];
        this.OriginPos3 = this.OrderPos[2];
        this.OrderCounterOFF();
        this.AnswerResultOFF();
        this.HukidashiOFF();
        this.NextGameFlg = true;
        this.TapText.text = "タップしてスタート！";

        // BGM
        SoundManager.Instance.PlayBGM(BGMName.DrinkingParty);

        // クリア条件の設定
        if (this.GameMode)
        {
            this.ClearQuota = (int)(this.Limit * 0.8f) * this.HukidashiList.Length;
        }
        else
        {
            this.ClearQuota = (int)(this.Limit * 0.8f);
        }
        this.ClearScore = 0;
    }

    // 飲み会のゲームクリア判定
    public void GameResult()
    {
        if(this.ClearScore >= this.ClearQuota)
        {
            Common.Instance.clearFlag[Common.Instance.miniNum] = true;
        }
        else
        {
            Common.Instance.clearFlag[Common.Instance.miniNum] = false;
        }
    }

    // デバッグ用の答えカンニング
    void LookAnswer()
    {
        for(int i = 0; i < this.AnswerCheck.Length; i++)
        {
            switch (this.OrderBox[i])
            {
                case 0:
                    this.AnswerCheck[i] = "ソフトクリーム " + this.OrderCounter[i].ToString() + "つ";
                    break;
                case 1:
                    this.AnswerCheck[i] = "ワイン " + this.OrderCounter[i].ToString() + "つ";
                    break;
                case 2:
                    this.AnswerCheck[i] = "ハイボール " + this.OrderCounter[i].ToString() + "つ";
                    break;
                case 3:
                    this.AnswerCheck[i] = "アイス " + this.OrderCounter[i].ToString() + "つ";
                    break;
                case 4:
                    this.AnswerCheck[i] = "うぃんぽて " + this.OrderCounter[i].ToString() + "つ";
                    break;
                case 5:
                    this.AnswerCheck[i] = "えだまめ " + this.OrderCounter[i].ToString() + "つ";
                    break;
                case 6:
                    this.AnswerCheck[i] = "ウィスキー " + this.OrderCounter[i].ToString() + "つ";
                    break;
                case 7:
                    this.AnswerCheck[i] = "りんごてぃー " + this.OrderCounter[i].ToString() + "つ";
                    break;
                case 8:
                    this.AnswerCheck[i] = "からあげ " + this.OrderCounter[i].ToString() + "つ";
                    break;
                case 9:
                    this.AnswerCheck[i] = "サラダ " + this.OrderCounter[i].ToString() + "つ";
                    break;
                case 10:
                    this.AnswerCheck[i] = "なすび " + this.OrderCounter[i].ToString() + "つ";
                    break;
                case 11:
                    this.AnswerCheck[i] = "からぽて " + this.OrderCounter[i].ToString() + "つ";
                    break;
                case 12:
                    this.AnswerCheck[i] = "ビール " + this.OrderCounter[i].ToString() + "つ";
                    break;
                case 13:
                    this.AnswerCheck[i] = "チヂミ " + this.OrderCounter[i].ToString() + "つ";
                    break;
            }
        }
    }

	void Update () {
        if(Input.GetMouseButtonDown(0) && this.NextGameFlg)
        {
            if(Limit > 0)
            {
                this.TapText.gameObject.SetActive(false);
                this.NextGameFlg = false;
                this.AnswerResultOFF();
                this.HukidashiOFF();
                this.GameStart();
            }
            // ゲーム終了時の処理
            else
            {
                this.NextGameFlg = false;
                this.GameResult();
                Common.Instance.ChangeScene(Common.SceneName.Result);
            }
            
        }
        this.LimitText.text = this.Limit.ToString() + " 回";
    }
}


