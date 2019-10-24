using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class Mushikui {

    /// <summary>
    /// データ作成
    /// </summary>
    public struct MushikuiData
    {
        private string musikui; 
        private string question;
        private string answer;
        private string[] select;

       

        public MushikuiData(string message)
        {
            musikui = "";
            question = "";
            answer = "";
            
            select = new string[4];

            var messageList = message.Split(',');
            for(int i = 0; i < messageList.Length; i++)
            {
                if(i == 0)
                {
                    LoadMessage(messageList[i]);
                }
                else
                {
                    select[i - 1] = messageList[i];
                }
            }
        }

        //「」内の文字を抽出して置き換え
        private void LoadMessage(string message)
        {
            // mushikui を検出
            var startPos = message.IndexOf("「");
            var endPos = message.IndexOf("」");
            musikui = message.Substring(startPos + 1, endPos - startPos - 1);
            // mMessage を作成
            question = message.Replace("「" + musikui + "」", "＿＿＿");
            answer = message.Replace("「" + musikui + "」", musikui);
        }


        // 「」の中身取得
        public string Musikui { get { return musikui; } }
        // 問題文
        public string Question { get { return question; } }
        // 回答文
        public string Answer { get { return answer; } }
        // 選択肢取得
        public string[] Select { get { return select; } }

    }
    
    public Mushikui(string filepath)
    {
        Load(filepath);
    }

    private TextAsset CSVFile;
    public MushikuiData[] data;

    // Use this for initialization
    public void Load(string filepath)
    {
        // 外部からのファイルパスを使用
        CSVFile = Resources.Load(filepath) as TextAsset;
        StringReader render = new StringReader(CSVFile.text);
        data = new MushikuiData[render.Peek()];
        int num = 0;
        while (render.Peek() > -1)
        {
            string line = render.ReadLine();
            var musi = new MushikuiData(line);
            data[num] = musi;
            num++;
        }
    }
}
