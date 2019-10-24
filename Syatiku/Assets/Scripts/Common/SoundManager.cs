using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BGMName
{
    Boss,
    Hacking,
    Title,
    DrinkingParty,
    Smoking,
    Action,
    BadEnd,
    NormalEnd,
    GoodEnd,
    Scenario,
}

public enum SEName
{
    DenmokuTap,     //飲み会デンモクタップ
    Cancel,         //戻るボタン系
    CorrectChoice,  //喫煙で正解選択
    EnemyCome,      //ハッキングで敵(社長、課長)が来たとき
    FrameFell,      //ハッキングで額縁が落ちた時
    FindInfo,       //ハッキングで資料、パスワードが見つかったとき
    Flick,          //テキストフリック
    Harisen,        //ハリセン
    CorrectHit,     //正解のテキスト衝突
    WrongChoice,    //喫煙で不正解選択
    Locker,         //ハッキングの引き出しあけたとき
    Message,        //シナリオ中タップ音
    Failed ,        //リザルト失敗
    Success,        //リザルト成功
    WrongHit,       //不正解テキスト衝突
    PasswordMiss,   //パスワードミスった
    Page,           //ハッキングのペラペラ
    SetPassword,    //パスワードの文字をはめるとき
    TapAction,      //ミニゲーム選択音
    Timer,          //喫煙所の選択肢表示中のタイマー音
    Hukidashi,      //リザルトの吹き出し音
    Impact,         //ボスドアップ音
    FootSound,      //ボス足音
    Spot2,          //スポットライト音
    Warning,        //ボス決戦アラート
    Menu,
    Joy,
    Angry,
    Sigh,
    Worry,
    Shock,
    Impatience,
    Question,
}

public enum SmokingVoiceName
{
    Start_man1,     //お疲れ様です
    Start_man2,     //じつは...
    Miss_man1,      //なにいってるんだ?
    Miss_man2,      //そんな...
    Failed_shirota, //そんな...
    Clear_shirota,  //やった!
    Start_woman,    //お疲れ様です
    Miss_woman,     //なにいってるの?
}

public class SoundManager : MonoBehaviour {

    static SoundManager instance;
    public static SoundManager Instance {
        get
        {
            if (instance != null) return instance;
            instance = FindObjectOfType<SoundManager>();
            return instance;
        }
    }

    void Awake()
    {
        CheckInstance();
        DontDestroyOnLoad(this);
    }

    void CheckInstance()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }
        if (instance == this)
        {
            return;
        }

        Destroy(gameObject);
    }

    [SerializeField]
    CriAtomSource bgmSource;
    [SerializeField]
    CriAtomSource seSource;
    [SerializeField]
    CriAtomSource voiceSource;
    int currentSeIndex;

    public void PlayBGM(BGMName cueName)
    {
        bgmSource.Stop();
        bgmSource.Play((int)cueName);
    }

    public void PlayBGM(string cueName)
    {
        bgmSource.Stop();
        bgmSource.Play(cueName);
    }

    public void PlaySE(SEName cueName)
    {
        //Timerが鳴っていたら止める
        if (currentSeIndex == 20)
        {
            seSource.Stop();
        }
        currentSeIndex = (int)cueName;
        seSource.Play(currentSeIndex);
    }

    public void PlaySE(string cueName)
    {
        seSource.Play(cueName);
    }

    public void PlayVoice(int cueId)
    {
        voiceSource.Play(cueId);
    }

    public void PlayVoice(string cueName)
    {
        voiceSource.Play(cueName);
    }

    public void SetVoiceSource(CriAtomSource source)
    {
        voiceSource = source;
        voiceSource.volume = 2f;
    }

    public bool IsVoiceEndOrStop()
    {
        return (voiceSource.status == CriAtomSource.Status.PlayEnd || voiceSource.status == CriAtomSource.Status.Stop);
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void StopSE()
    {
        seSource.Stop();
    }

    public void StopVoice()
    {
        if(voiceSource != null) voiceSource.Stop();
    }

}
