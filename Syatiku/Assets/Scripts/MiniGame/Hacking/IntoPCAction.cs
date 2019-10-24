using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IntoPCAction : MonoBehaviour {

    [SerializeField, Tooltip("PCPassWordフェーズ PC内のでる場所")]
    private GameObject[] PC_PassWordObject;
    [SerializeField, Tooltip("WindowPassWordフェーズ PC内のでる場所")]
    private GameObject[] Window_PassWordObject;
    [SerializeField, Tooltip("資料比較グループObject")]
    private GameObject Comparisoning;
    [SerializeField, Tooltip("残り回数更新Text")]
    private Text CountText;
    [SerializeField, Tooltip("見つけた資料")]
    private GameObject Document_1;
    [SerializeField, Tooltip("資料見つけてない場合のテキストObject")]
    private GameObject NotComp;
    [SerializeField, Tooltip("資料比較するボタン")]
    private GameObject comp_btn;
    [SerializeField, Tooltip("間違っている箇所_0")]
    private GameObject wrongbtn_0;
    [SerializeField, Tooltip("間違っている箇所_1")]
    private GameObject wrongbtn_1;
    [SerializeField, Tooltip("EventSystem")]
    private EventSystem event_system;
    [SerializeField, Tooltip("FolderパスワードのResultText")]
    private GameObject folder_text;
    [SerializeField, Tooltip("WindowFase戻るボタン")]
    private GameObject window_returnbtn;

    [Tooltip("資料比較の時に何回ミスしてもいいかの回数")]
    public int tappingCount = 6;
    [SerializeField, Tooltip("成功エフェクト")]
    private GameObject cleareffect;

    private Transform password_child;

    private HackMain hack_main;
    private HackTap hack_tap;
    private GameObject PC_login;
    private GameObject WindowFase;
    private GameObject Window;
    private GameObject PassWordFase;
    private GameObject[] PassWordObject;

    [HideInInspector]
    public bool _compariClear = false;

    //資料比較の時の間違っている部分をタップできたかどうか
    private bool doc_0 = false;
    private bool doc_1 = false;

    [HideInInspector]
    public bool _isWindowAnim = false;

    // Use this for initialization
    void Start() {
        try
        {
            PC_login = GameObject.Find("Canvas/PC/PassWordFase/Title");
            Window = GameObject.Find("Canvas/PC/WindowFase/Window");
            WindowFase = GameObject.Find("Canvas/PC/WindowFase");
            PassWordFase = GameObject.Find("Canvas/PC/PassWordFase");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        _compariClear = false;
        PassWordFase.transform.SetSiblingIndex(2);
        tappingCount = 6;
        CountText.text = tappingCount.ToString();
        Window.SetActive(false);
        Document_1.SetActive(false);
        NotComp.SetActive(true);
        Comparisoning.SetActive(false);
        comp_btn.SetActive(true);
        folder_text.SetActive(false);
        hack_main = GetComponent<HackMain>();
        hack_tap = GetComponent<HackTap>();
        doc_0 = false;
        doc_1 = false;
        _isWindowAnim = false;
    }

    /// <summary>
    /// チェックした時のテキスト表示
    /// </summary>
    /// <param name="wait">待つ時間をfloat型で記入しなはれ</param>
    /// <returns></returns>
    private IEnumerator PcLogin_WaitTime(bool _isResult)
    {
        if (_isResult)
        {
            cleareffect.SetActive(true);
            SoundManager.Instance.PlaySE(SEName.TapAction);
            PC_login.GetComponent<Text>().text = "ログインできました。";
            PC_login.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            hack_tap._windowFase = true;
            WindowFase.transform.SetSiblingIndex(2);
        }
        else
        {
            SoundManager.Instance.PlaySE(SEName.PasswordMiss);
            PC_login.GetComponent<Text>().text = "パスワードが違います。";
            PC_login.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            PC_login.SetActive(false);
        }
    }

    /// <summary>
    /// フォルダフェーズでログインできたかどうかの処理
    /// </summary>
    /// <param name="_isResult">true=ログイン成功、false=ログインできてない</param>
    /// <returns></returns>
    private IEnumerator FolderLogin_WaitTime(bool _isResult)
    {
        if (_isResult)
        {
            cleareffect.SetActive(true);
            SoundManager.Instance.PlaySE(SEName.TapAction);
            _isWindowAnim = true;
            hack_tap.ZoomActive(7);
            folder_text.GetComponent<Text>().text = "ログインできました。";
            folder_text.SetActive(true);
            Window.SetActive(true);
            window_returnbtn.SetActive(false);
            yield return new WaitForSeconds(1.5f);
            window_returnbtn.SetActive(true);
            folder_text.SetActive(false);
        }
        else
        {
            SoundManager.Instance.PlaySE(SEName.PasswordMiss);
            folder_text.GetComponent<Text>().text = "パスワードが違います。";
            folder_text.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            folder_text.SetActive(false);
        }
        _isWindowAnim = false;
    }
    
    /// <summary>
    /// 資料比較する時の処理
    /// </summary>
    public void DocumentsComparison()
    {
        if (hack_tap._getDocument)
        {
            Document_1.SetActive(true);
            NotComp.SetActive(false);
        }
        Comparisoning.SetActive(true);
    }

    /// <summary>
    /// 資料比較中の周りの何もない部分タップ処理
    /// </summary>
    public void OutTap()
    {
        Comparisoning.SetActive(false);
    }

    /// <summary>
    /// 資料比較の時のタップ処理
    /// </summary>
    /// <param name="docNum">0.当たり 1.当たり 2.はずれ 3.何もないとこ</param>
    public void CheckDocuments(int docNum)
    {
        if (!hack_tap._getDocument)
            return;
        switch (docNum)
        {
            case 0:
                wrongbtn_0.GetComponent<Image>().color = new Color(255, 0, 0, 0.5f);
                doc_0 = true;
                break;
            case 1:
                wrongbtn_1.GetComponent<Image>().color = new Color(255, 0, 0, 0.5f);
                doc_1 = true;
                break;
            case 2:
                tappingCount--;
                CountText.text = tappingCount.ToString();
                break;
        }

        if(doc_0 && doc_1)
        {
            comp_btn.SetActive(false);
            _compariClear = true;
            Common.Instance.dataFlag[2] = true;
            Common.Instance.dataFlag[3] = true;
            Common.Instance.clearFlag[Common.Instance.miniNum] = true;
            Common.Instance.ChangeScene(Common.SceneName.Result);
            OutTap();
        }

        if(tappingCount == 0)
        {
            Common.Instance.dataFlag[2] = false;
            Common.Instance.clearFlag[Common.Instance.miniNum] = true;
            Common.Instance.ChangeScene(Common.SceneName.Result);
        }
    }

    /// <summary>
    /// PCパスワードフェーズパスワードチェック
    /// </summary>
    /// <param name="_pcpass">true=PCパスワードフェーズ, false=Folderパスワードフェーズ</param>
    public void CheckPassWord(bool _pcpass)
    {
        List<string> Ans_list = new List<string>();
        if (_pcpass)
        {
            PassWordObject = PC_PassWordObject;
            Ans_list = hack_main.Quest_list;
        }
        else
        {
            PassWordObject = Window_PassWordObject;
            Ans_list = hack_main.Folder_ans_list;
        }
        for(int i=0; i < PassWordObject.Length; i++)
        {
            GameObject password_parent;
            if (_pcpass)
                password_parent = GameObject.Find("Canvas/PC/PassWordFase/PassWord/Password_" + i);
            else
                password_parent = GameObject.Find("Canvas/Zoom/AdminStrator/AdminPage/AdminPassWord/Password_" + i);

            if (PassWordObject[i].gameObject.transform.childCount != 0)
                password_child = password_parent.transform.GetChild(0).GetChild(0);
            else
            {
                if (_pcpass)
                    StartCoroutine(PcLogin_WaitTime(false));
                else
                    StartCoroutine(FolderLogin_WaitTime(false));
                break;
            }
            Text child_text = password_child.GetComponent<Text>();
            if(child_text.text.Substring(0,1) == Ans_list[i].ToString())
            {
                if(i == PassWordObject.Length - 1)
                {
                    if (_pcpass)
                        StartCoroutine(PcLogin_WaitTime(true));
                    else
                        StartCoroutine(FolderLogin_WaitTime(true));
                }
            }
            else
            {
                if (_pcpass)
                    StartCoroutine(PcLogin_WaitTime(false));
                else
                    StartCoroutine(FolderLogin_WaitTime(false));
                break;
            }
        }
    }
}
