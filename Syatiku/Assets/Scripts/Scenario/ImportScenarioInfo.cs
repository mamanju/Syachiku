using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using DG.Tweening;

public class ImportScenarioInfo : MonoBehaviour {

    ScenarioWindow window;
    int voiceCount;

    public ImportScenarioInfo(string filePath, ref List<ScenarioInfo> scenarioList, ScenarioWindow window, int startVoiceIndex)
    {
        this.window = window;
        voiceCount = startVoiceIndex;
        List<ScenarioInfo> scenarioInfos = new List<ScenarioInfo>();
        //テキストファイルの読み込み
        TextAsset textAsset = Resources.Load<TextAsset>(filePath);

        //@brでシナリオを区切る
        string[] scenarios = textAsset.text.Split(new string[] { "@br" }, System.StringSplitOptions.None);

        for (int i = 0; i < scenarios.Length; i++)
        {
            scenarioInfos.Add(ScenarioAnalysis(scenarios[i]));
        }

        scenarioList = scenarioInfos;
    }

    /// <summary>
    /// 区切ったシナリオを細かく解析
    /// </summary>
    ScenarioInfo ScenarioAnalysis(string line)
    {
        ScenarioInfo scenario = new ScenarioInfo();

        StringReader sr = new StringReader(line);
        var message = new System.Text.StringBuilder();

        string text;
        while ((text = sr.ReadLine()) != null)
        {

            if (text.StartsWith("//") || string.IsNullOrEmpty(text))
            {
                //不必要な行
                continue;
            }
            else if (text.StartsWith("@"))
            {
                //コマンドの行
                ParseCommandInfo(text, scenario);
            }
            else
            {
                //セリフの行
                message.Append(text + "\n");
            }
        }

        if (message.Length != 0)
        {
            scenario.message = message.ToString();
        }

        return scenario;
    }

    #region ParseCommand

    /// <summary>
    /// コマンドによって処理を分ける
    /// </summary>
    void ParseCommandInfo(string text, ScenarioInfo scenario)
    {
        if (text.Contains("name"))
        {
            scenario.commandActionList.Add(() =>
            {
                //名前
                window.name.text = TakeTextInfo(text) ?? "";
                int pos = GetTargetPosNum(text);
                ShadeOffCharacters(pos);
            });
        }
        else if (text.Contains("charaOn"))
        {
            //キャラクター画像表示
            float time = GetTime(text);
            scenario.commandActionList.Add(() =>
            {
                string imagePath = "Scenario/" + TakeTextInfo(text);
                Image target = GetTargetImage(text);
                SetSprite(target, imagePath);
                target.gameObject.SetActive(true);
                FadeManager.Instance.Fade(target, time, 1f);
            });
            if (time > 0) scenario.fadeTimeList.Add(scenario.commandActionList.Count - 1, time);
        }
        else if (text.Contains("emo"))
        {
            //感情アイコン生成
            scenario.commandActionList.Add(() =>
            {
                string emotion = TakeTextInfo(text);
                int type = GetEmotionTypeNum(emotion);
                int pos = GetTargetPosNum(text);
                CreateEmotion(type, pos);
                switch (type)
                {
                    case 0:
                    case 1:
                    case 7:
                        ActiveMotion(pos, 30f, 0.5f);
                        break;
                    case 4:
                    case 5:
                    case 6:
                        ActiveMotion(pos, -30f, 0.5f);
                        break;
                    default:
                        ActiveMotion(pos, -30f, 1.5f);
                        break;
                }
            });
        }
        else if (text.Contains("charaOff"))
        {
            //キャラクター画像非表示
            float time = GetTime(text);
            scenario.commandActionList.Add(() =>
            {
                Image target = GetTargetImage(text);
                target.gameObject.SetActive(false);
                FadeManager.Instance.Fade(target, time, 0);
            });
            if(time > 0) scenario.fadeTimeList.Add(scenario.commandActionList.Count - 1, time);
        }
        else if (text.Contains("se"))
        {
            //SE
            scenario.commandActionList.Add(() =>
            {
                string cueName = TakeTextInfo(text);
                if (string.IsNullOrEmpty(cueName)) SoundManager.Instance.StopBGM();
                else SoundManager.Instance.PlaySE(cueName);
            });
        }
        else if (text.Contains("bgm"))
        {
            //BGM
            scenario.commandActionList.Add(() =>
            {
                string cueName = TakeTextInfo(text);
                if (string.IsNullOrEmpty(cueName)) SoundManager.Instance.StopSE();
                else SoundManager.Instance.PlayBGM(cueName);
            });
        }
        else if (text.Contains("cv"))
        {
            //Voice
            scenario.commandActionList.Add(() =>
            {
                int voiceNum = TakeVoiceNum(text);
                //シナリオ用
                if (voiceNum == -1)
                {
                    SoundManager.Instance.PlayVoice(voiceCount++);
                }
                //喫煙用
                else
                {
                    SoundManager.Instance.PlayVoice(voiceNum);
                }
            });
        }
        else if (text.Contains("end"))
        {
            scenario.commandActionList.Add(() =>
                FadeManager.Instance.Fade(window.scenarioCanvas, GetTime(text), 0f)
            );
        }
        else if (text.Contains("scene"))
        {
            scenario.commandActionList.Add(() =>
            {
                string sceneName = TakeTextInfo(text);
                //Common.Instance.Fade();
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                window.Init();
            });
        }
        else if (text.Contains("bgi"))
        {
            //背景画像
            scenario.commandActionList.Add(() =>
            {
                string imageName = TakeTextInfo(text);
                if (imageName == "b")
                {
                    window.bgi.color = Color.black;
                }
                else
                {
                    string imagePath = "Scenario/" + imageName;
                    SetSprite(window.bgi, imagePath);
                    window.bgi.color = Color.white;
                }
            });
        }
        else if (text.Contains("goto"))
        {
            //指定した所まで情報の更新
            scenario.commandActionList.Add(() =>
            {
                string infoIndex = TakeTextInfo(text);
                ScenarioController.Instance.infoIndex = int.Parse(infoIndex);
                voiceCount = TakeVoiceNum(text);
            });
        }
        else if (text.Contains("change"))
        {
            //アナザーエピローグの例外処理
            scenario.commandActionList.Add(() =>
            {
                RectTransform leftRect = window.characters[2].GetComponent<RectTransform>();
                leftRect.localPosition = new Vector3(540f, -390f, 0);
                leftRect.localScale = Vector3.one * 1.3f;
                leftRect.sizeDelta = new Vector2(800f, 1200f);
            });
        }
    }

    /// <summary>
    /// テキストの情報を抜き取る ({ }の中身)
    /// </summary>
    string TakeTextInfo(string text)
    {
        int beginNum = text.IndexOf("{") + 1;
        int lastNum = text.IndexOf("}");
        return text.Substring(beginNum, lastNum - beginNum);
    }

    /// <summary>
    /// テキストのボイス番号を抜き取る ([ ]の中身)
    /// </summary>
    int TakeVoiceNum(string text)
    {
        int beginNum = text.IndexOf("[") + 1;
        int lastNum = text.IndexOf("]");
        int voiceNum = -1;
        if (beginNum > 0 && lastNum > 0)
        {
            int.TryParse(text.Substring(beginNum, lastNum - beginNum), out voiceNum);
        }
        return voiceNum;
    }

    /// <summary>
    /// キャラクターの色合い変更
    /// </summary>
    /// <param name="pos"></param>
    void ShadeOffCharacters(int pos)
    {
        for (int i = 0; i < window.characters.Length; i++)
        {
            if (!window.characters[i].gameObject.activeSelf) continue;
            if (i == pos) window.characters[i].color = Color.white;
            else window.characters[i].color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }

    /// <summary>
    /// 画像をセット
    /// </summary>
    void SetSprite(Image image, string path)
    {
        image.sprite = Resources.Load<Sprite>(path);
    }

    /// <summary>
    /// 対象画像を取得
    /// </summary>
    Image GetTargetImage(string text)
    {
        Image target = null;
        int pos = GetTargetPosNum(text);
        if (pos >= 0)
        {
            target = window.characters[pos];
        }
        return target;
    }

    /// <summary>
    /// 指定したイメージの位置番号を返す（0: 左 1:中央 2: 右）
    /// </summary>
    int GetTargetPosNum(string text)
    {
        if (text.LastIndexOf("left") >= 0)
        {
            return 0;
        }
        else if (text.LastIndexOf("center") >= 0)
        {
            return 1;
        }
        else if (text.LastIndexOf("right") >= 0)
        {
            return 2;
        }

        return -1;
    }

    int GetEmotionTypeNum(string text)
    {
        switch (text)
        {
            case "joy":
                return 0;
            case "angry":
                return 1;
            case "shock":
                return 2;
            case "sigh":
                return 3;
            case "question":
                return 4;
            case "worry":
                return 5;
            case "impatience":
                return 6;
        }
        return -1;
    }

    void ActiveMotion(int posNum, float jumpNum, float time)
    {
        RectTransform character = window.characters[posNum].GetComponent<RectTransform>();
        character.DOLocalJump(
            character.localPosition,
            jumpNum,
            1,
            time
        );
    }

    /// <summary>
    /// 感情アイコンの生成
    /// </summary>
    /// <param name="emotionNum">感情アイコンの番号</param>
    /// <param name="posNum">位置番号</param>
    void CreateEmotion(int emotionNum, int posNum)
    {
        if (emotionNum < 0 || posNum < 0) return;
        GameObject emotion = Instantiate(window.emotionPrefabs[emotionNum], window.emotionsParent);
        emotion.GetComponent<RectTransform>().localPosition = window.emotionPosList[emotionNum].list[posNum];
    }

    /// <summary>
    /// 時間を取得
    /// </summary>
    float GetTime(string text)
    {
        int beginNum = text.IndexOf("[") + 1;
        int lastNum = text.IndexOf("]");
        if (beginNum < 1 || lastNum < 0) return 0;
        string timeString = text.Substring(beginNum, lastNum - beginNum);
        float time;
        //中身がなければ0を返す
        float.TryParse(timeString, out time);

        return time;
    }
    
    #endregion
}
