using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DenmokuMeter : MonoBehaviour {

    // インスタンスの取得
    ButtonController button;

    // デンモクの制限時間メーター
    public Slider TimeMeter;

    // メーターの有効・無効フラグ
    [HideInInspector]
    public bool TimeMeterFlg;

    // デンモクの制限時間
    [Range(1, 60), Tooltip("デンモクの制限時間(秒)")]
    public float Timer;

    // 時間切れのフラグ
    [HideInInspector]
    public bool TimeOverFlg;

	void Start () {
        this.TimeMeter.maxValue = Timer;　　// メーターの最大値の設定
        this.TimeMeter.value = 0; 　// デンモクの制限時間の設定
        this.button = GetComponent<ButtonController>();
	}
	
	
	void Update () {
        if (TimeMeterFlg)
        {
            TimeMeter.value += Time.deltaTime;
            
            // 時間切れの処理
            if(this.TimeMeter.value == this.TimeMeter.maxValue)
            {
                this.TimeOverFlg = true;
                button.Order();
            }
        }
	}
}
