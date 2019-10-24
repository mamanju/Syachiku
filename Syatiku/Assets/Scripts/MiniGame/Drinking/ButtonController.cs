using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {

    // スクリプトのインスタンスの取得
    DrinkScene drink;
    Denmoku denmoku;
    DenmokuMeter meter;

    // 覚えたボタン、もう一度ボタン
    public Button Remember;
    public Button Again;

    // メニュータブのボタン
    public Button Otsumami;
    public Button Drink;
    public Button Dessert;

    // 注文ボタン
    public Button OrderButton;

    // 注文するメニューのボタン
    public Button[] Menu;
    
    // デンモクのGameObject取得
    [SerializeField]
    Image DenmokuImage;

    // メニューのスクロールするコンテンツ
    [SerializeField]
    GameObject Menu_Otsumami, Menu_Drink, Menu_Dessert;

    // デンモクのスクロールバー
    [SerializeField]
    Scrollbar Scrollbar_Otsumami;

    [SerializeField]
    Scrollbar Scrollbar_Drink;

    // 注文数をカウントする
    [HideInInspector]
    public int OrderCount = 0;

    // もう一度ボタンが押されたかの判定をする為のフラグ
    private bool AgainFlg = true;

    [HideInInspector]
    public int CounterNum;

    // 飲み会シーンのボタンを表示
    public void DrinkSceneButton(bool b)
    {
        if (b)
        {
            this.Remember.gameObject.SetActive(b);
            
            // もう一度注文を聞くボタンが押されたかの判定
            if (this.AgainFlg)
            {
                this.Again.gameObject.SetActive(b);
                this.Remember.GetComponent<RectTransform>().localPosition = new Vector2(0, 50);
            }
            else
            {
                this.Remember.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            }
        }
        else
        {
            this.Remember.gameObject.SetActive(b);
            this.Again.gameObject.SetActive(b);
        }
    }

    // 覚えたボタン
    public void RememberButton()
    {
        this.AgainFlg = true;
        this.DrinkSceneButton(false);
        this.OtsumamiButton();
        denmoku.MenuListOFF();
        meter.TimeMeterFlg = true;
    }

    // もう一度注文を聞くボタン
    public void AgainButton()
    {
        this.AgainFlg = false;
        this.DrinkSceneButton(false);
        StartCoroutine(drink.OrderMethod());
    }

    // メニュータブのおつまみボタン
    public void OtsumamiButton()
    {
        this.MenuTabControll(false, true, true, true, false, false);
    }

    // メニュータブの飲み物ボタン
    public void DrinkButton()
    {
        this.MenuTabControll(true, false, true, false, true, false);
    }

    // メニュータブのデザートボタン
    public void DessertButton()
    {
        this.MenuTabControll(true, true, false, false, false, true);
    }

    // デンモクのメニュータブのボタン管理
    public void MenuTabControll(bool b1, bool b2, bool b3, bool b4, bool b5, bool b6)
    {
        this.Scrollbar_Otsumami.value = 0;
        this.Scrollbar_Drink.value = 0;
        this.Otsumami.interactable = b1;
        this.Drink.interactable = b2;
        this.Dessert.interactable = b3;
        this.Menu_Otsumami.gameObject.SetActive(b4);
        this.Menu_Drink.gameObject.SetActive(b5);
        this.Menu_Dessert.gameObject.SetActive(b6);
    }

    // デンモクのメニューボタン
    public void MenuButton(int i)
    {
        if (this.OrderCount != 4)
        {
            SoundManager.Instance.PlaySE(SEName.DenmokuTap);
            this.Menu[i].interactable = false;
            denmoku.ListInMenu(i);
            this.OrderCount++;
        }
    }

    // 注文ボタン
    public void Order()
    {
        this.ButtonReset();
        this.OrderCount = 0;
        this.DenmokuImage.transform.localPosition = new Vector2(0, -1500);
        drink.Delete();
        StartCoroutine(drink.Answer());
        meter.TimeMeterFlg = false;
        meter.TimeMeter.value = 0;
    }
    
    // メニューのボタンを有効にする
    public void ButtonReset()
    {
        for(int i = 0; i < denmoku.InputOrderBox.Length; i++)
        {
            if(denmoku.InputOrderBox[i] >= 0)
            {
                this.Menu[denmoku.InputOrderBox[i]].interactable = true;
            }
        }
    }

    // 注文リストの個数カウンターボタン
    public void CounterController(bool b)
    {
        if (b)
        {
            if (denmoku.InputOrderCounter[this.CounterNum] < 4)
            {
                SoundManager.Instance.PlaySE(SEName.DenmokuTap);
                denmoku.InputOrderCounter[this.CounterNum]++;
            }
        }
        else
        {
            SoundManager.Instance.PlaySE(SEName.DenmokuTap);

            if (denmoku.InputOrderCounter[this.CounterNum] > 1)
            {
                denmoku.InputOrderCounter[this.CounterNum]--;
            }
            else
            {
                for(int i = 0; i < denmoku.InputOrderBox.Length; i++)
                {
                    if(this.Menu[denmoku.InputOrderBox[this.CounterNum]].interactable == false)
                    {
                        this.Menu[denmoku.InputOrderBox[this.CounterNum]].interactable = true;
                        break;
                    }
                }
                denmoku.OrderListArrange();
                if(this.OrderCount > 0)
                {
                    this.OrderCount--;
                }
            }
        }
    }

    // 何番目の注文リストの個数カウンターを変更するかを決める
    public void CounterButton(int i)
    {
        if(this.CounterNum != i)
        {
            this.CounterNum = i;
        }
    }

    // ボタンを押したときにSEを再生
    public void StartSE()
    {
        SoundManager.Instance.PlaySE(SEName.DenmokuTap);
    }

    void Start () {
        drink = GetComponent<DrinkScene>();
        denmoku = GetComponent<Denmoku>();
        meter = GetComponent<DenmokuMeter>();
        this.DrinkSceneButton(false);
        this.DenmokuImage.transform.localPosition = new Vector2(0, -1500);
    }
	
	
	void Update () {
        // 注文ボタンの有効・無効の管理
        if (this.OrderCount == 4)
        {
            this.OrderButton.interactable = true;
        }
        else
        {
            this.OrderButton.interactable = false;
        }
    }
}
