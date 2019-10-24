using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleController : MonoBehaviour {
    [SerializeField]
    private GameObject title,modeText; // タイトル文字UI、ゲームモードテキスト

    void Awake()
    {
        // Commonがなければ生成
        if (!Common.Instance)
        {
            var common = Instantiate(Resources.Load("Prefabs/Common/Common"));
            DontDestroyOnLoad(common);
        }
    }

    void Start() {
        SoundManager.Instance.PlayBGM(BGMName.Title); // BGN再生
        Common.Instance.Init(); // 各数値初期化
    }

    //モード選択
    public void ChangeMode(int mode)
    {
        Common.Instance.gameMode = mode;
        if (mode == 1)
        {
            Common.Instance.actionCount = 3; // WHITE
            Common.Instance.ChangeScene(Common.SceneName.MainEpilogue);
        }
        else
        {
            Common.Instance.actionCount = 1; // ANOTHER
            Common.Instance.ChangeScene(Common.SceneName.AnotherEpilogue);
        }

        
    }
    //タイトルボタンを削除
    public void Select()
    {
        title.SetActive(false);
        modeText.SetActive(true);
    }

}
