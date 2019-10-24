using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioScene : MonoBehaviour {

    [SerializeField]
    string filePath;
    [SerializeField]
    CriAtomSource voiceSource;

    void Start () {
        //現在のシーン名を取得
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.SetVoiceSource(voiceSource);
        //決戦前のシナリオの場合
        if (sceneName == "BeforeBattle")
        {
            //White
            if (Common.Instance.gameMode == 1)
            {
                BeginScenarioMainBeforeBattle();
            }
            //Another
            else
            {
                BeginScenarioAnotherBeforeBattle();
            }
        }
        else
        {
            ScenarioController.Instance.BeginScenario(filePath);
        }
    }

    /// <summary>
    /// ボス決戦前のメインシナリオ再生(シナリオを始める番号:最初のボイス番号)
    /// vs社長(bad 0:0 normal 6:1 good 11:2)
    /// </summary>
    private void BeginScenarioMainBeforeBattle()
    {
        int startInfoIndex = 0;
        int startVoiceIndex = 0;
        bool[] clearFlag = Common.Instance.clearFlag;
        int clearCount = 0;
        //クリアしたミニゲームの数
        for (int i = 0; i < clearFlag.Length; i++)
        {
            clearCount += clearFlag[i] ? 1 : 0;
        }
        switch (clearCount)
        {
            //Bad
            case 0:
            case 1:
                startInfoIndex = 11;
                startVoiceIndex = 2;
                break;
            //Normal
            case 2:
                startInfoIndex = 6;
                startVoiceIndex = 1;
                break;
            //Good
            case 3:
                break;
        }
        ScenarioController.Instance.BeginScenario(filePath, startInfoIndex, startVoiceIndex);
    }

    /// <summary>
    /// ボス決戦前のアナザーシナリオ再生(シナリオを始める番号:最初のボイス番号)
    /// vs社長(bad,normal 0:0 good 4:1)
    /// vs課長
    /// </summary>
    private void BeginScenarioAnotherBeforeBattle()
    {
        int startInfoIndex = 0;
        int startVoiceIndex = 0;
        bool[] clearFlag = Common.Instance.clearFlag;
        int clearCount = 0;
        //クリアしたミニゲームの数
        for (int i = 0; i < clearFlag.Length; i++)
        {
            clearCount += clearFlag[i] ? 1 : 0;
        }
        switch (clearCount)
        {
            //Bad or Normal
            case 0:
                startVoiceIndex = 1;
                break;
            //Good
            case 1:
                startInfoIndex = 4;
                break;
        }

        ScenarioController.Instance.BeginScenario(filePath, startInfoIndex, startVoiceIndex);
    }
}
