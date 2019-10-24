using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossText : MonoBehaviour {

    [SerializeField, Tooltip("ボスの質問Text")]
    private Text boss_textObject;
    [SerializeField, Tooltip("答えるボタンText_0")]
    private Text boss_textbtn_0;
    [SerializeField, Tooltip("答えるボタンText_1")]
    private Text boss_textbtn_1;

    private List<string> Boss_text = new List<string>();
    private string str_line;
    [HideInInspector]
    public string[] stren;

    private int maxLine = 0;
    private int currentLine = 0;

    // Use this for initialization
    void Start ()
    {
        maxLine = 0;
        currentLine = 0;
        ReadBossText();
    }
	
    /// <summary>
    /// ボスのテキスト読み込み
    /// </summary>
    private void ReadBossText()
    {
        TextAsset csv_file = Resources.Load("Minigame/Hacking/Hack_BossText") as TextAsset;
        System.IO.StringReader str_text = new System.IO.StringReader(csv_file.text);
        while(str_text.Peek() > -1)
        {
            str_line = str_text.ReadLine();
            Boss_text.Add(str_line);
            maxLine++;
        }
    }

    /// <summary>
    /// テキスト内容変更処理
    /// </summary>
    public void AddText()
    {
        if (currentLine > maxLine - 1)
            return;
            
        stren = Boss_text[currentLine].ToString().Split(',');
        boss_textObject.text = stren[0];
        boss_textbtn_0.text = stren[1];
        boss_textbtn_1.text = stren[2];
        currentLine++;
    }
}
