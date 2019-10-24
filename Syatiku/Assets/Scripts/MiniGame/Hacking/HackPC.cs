using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HackPC : MonoBehaviour {
    
    private Text prefabText;
    [HideInInspector]
    public int counter = 0;

    private bool _isResult = false;

    [SerializeField, Tooltip("OKを押した結果を表示するGameObject")]
    private GameObject result_temp;

    private Transform ans_child;

    // Use this for initialization
    void Start () {
        result_temp.SetActive(false);
        _isResult = false;
        ChippedString();
    }

    // Update is called once per frame
    void Update () {
        
	}

    /// <summary>
    /// 欠けてる文章の処理
    /// </summary>
    private void ChippedString()
    {
        
    }

    /// <summary>
    /// バラバラの文字処理
    /// </summary>
    private void PiecesString()
    {
        // 欠けてる文章によって出す文字変更
        // 場所を設定　ランダムでやるかも

    }

    private IEnumerator WaitTime(float time)
    {
        if (_isResult)
            result_temp.transform.GetComponentInChildren<Text>().text = "〇";
        else
            result_temp.transform.GetComponentInChildren<Text>().text = "×";

        result_temp.SetActive(true);
        yield return new WaitForSeconds(time);
        result_temp.SetActive(false);
    }

    public void CheckString()
    {
        for (int i = 0; i <= 5; i++)
        {
            GameObject ans = GameObject.Find("Canvas/IntoPC/Answer/AnswerText_" + i);
            GameObject quest = GameObject.Find("Canvas/IntoPC/Quest/QuestText_" + i);
            Text que_text = quest.GetComponent<Text>();
            if (ans.transform.childCount != 0)
                ans_child = ans.transform.GetChild(0).GetChild(0);
            else
            {
                _isResult = false;
                StartCoroutine(WaitTime(4f));
                break;
            }
            Text ansChild_text = ans_child.GetComponent<Text>();
            if (ansChild_text.text.Substring(0, ansChild_text.text.Length) == que_text.text)
            {
                if (i == 5)
                {
                    _isResult = true;
                    StartCoroutine(WaitTime(2.5f));
                    //Common.Instance.gameScore("Hacking",100);
                    Common.Instance.ChangeScene(Common.SceneName.Result);
                }
            }
            else
            {
                _isResult = false;
                StartCoroutine(WaitTime(2.5f));
                break;
            }
        }
    }
}
