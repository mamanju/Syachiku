using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 汎用スクリプト(Common.Instance.関数、変数名)
/// </summary>
public class Common : MonoBehaviour {

    /// <summary>
    /// シーン名
    /// </summary>
    public enum SceneName
    {
        Title = 0,
        Action,
        Smoking,
        Hacking,
        Drinking,
        Boss,
        Result,

        // Main---------------*
        MainEpilogue,
        Progress,
        BeforeBattle,
        MainGoodEnd,
        MainNormalEnd,
        MainBadEnd,
        // -------------------*

        // Another------------*
        AnotherEpilogue,
        AnotherBeforeBattle,
        AnotherGoodEnd,
        AnotherNormalEnd,
        AnotherBadEnd,
        //--------------------*
    }

    /// <summary>
    /// ミニゲームで手に入る資料
    /// </summary>
    [System.NonSerialized]
    public bool[] dataFlag =
    {
        false,
        false,
        false,
        false,
        false,
    };

    // ミニゲームクリアフラグ
    [System.NonSerialized]
    public bool[] clearFlag = 
    {
        false, // hack
        false, // drink
        false  // smoke
    };

    /// <summary>
    /// 何のミニゲームやったか(0:hack,1:drink,2:smoke)
    /// </summary>
    [System.NonSerialized]
    public int miniNum;

    [System.NonSerialized]
    public int gameMode; // シナリオがどちらか

    // 初期化しない変数-----------------------------
    [SerializeField,Header("シーン遷移時の時間")]
    private float interval; // シーン遷移時の時間
    private bool isFading = false;
    private Color fadeColor = Color.black;
    private float fadeAlpha = 0;
    private static Common instance;
    // --------------------------------------------

    [System.NonSerialized]
    public int actionCount; // 行動回数

    // 同じオブジェクト(Common)があるか判定
    public static Common Instance
    {
        get
        {
            if (instance == null) {
                instance = FindObjectOfType<Common>();
            }
            return instance;
        }
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init() {
        // dataFlag
        for (int i = 0; i < dataFlag.Length; i++) {
            dataFlag[i] = false;
        }
        // clearFlag
        for (int i = 0; i < clearFlag.Length; i++) {
            clearFlag[i] = false;
        }
        // gameMode
        gameMode = -1;
        // miniNum
        miniNum = -1;
        // actionCount
        actionCount = 0;
    }

    // フェードのUIを描画
    public void OnGUI()
    {
        if (this.isFading)
        {
            this.fadeColor.a = this.fadeAlpha;
            GUI.color = this.fadeColor;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// シーン遷移処理(Common.Instance.ChangeScene(Common.SceneName.シーン名))
    /// </summary>
    /// <param name="name"></param>
    public void ChangeScene(SceneName name)
    {
        StartCoroutine(Fade(name));
        var eventSystem = FindObjectOfType<EventSystem>();
        eventSystem.enabled = false;
    }

    /// <summary>
    /// フェード処理
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IEnumerator Fade(SceneName name)
    {
        this.isFading = true;
        float time = 0;
        
        while (time <= interval)
        {
            this.fadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }

        SceneManager.LoadScene((int)name);

        time = 0;
        while (time <= interval)
        {
            this.fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }
        this.isFading = false;
    }

    /// <summary>
    /// シャッフル変数(Common.Instance.Shuffle(配列))
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="param"></param>
    /// <returns></returns>
    public T[] Shuffle<T>(T[] param)
    {
        for (int i = 0; i < param.Length; i++)
        {
            T temp = param[i];
            int rand = Random.Range(0, param.Length - 1);
            param[i] = param[rand];
            param[rand] = temp;
        }
        return param;
    }
}
