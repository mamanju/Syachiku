using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HackGetWord : MonoBehaviour {

    //-------------------引き出しで使う変数---------------------------------------
    [Tooltip("集めた単語(Folder内に出すObject)")]
    private GameObject CollectFolderPrefab;
    [Tooltip("集めた単語(リスト内に出すObject)")]
    private GameObject GetWordFolderPrefab;

    private List<int> getDrawerNum = new List<int>();
    //-----------------------------------------------------------------------------

    //-------------------棚で使う変数----------------------------------------------
    [Tooltip("集めた単語(リスト内に出すObject)")]
    private GameObject GetWordPrefab;
    private GameObject GetWord;

    [Tooltip("集めた単語(PC内に出すObject)")]
    private GameObject CollectedPrefab;
    [Tooltip("集めた単語(PC内に出す場所)")]
    private GameObject CollectedWord;

    private List<int> getNum = new List<int>();
    //-----------------------------------------------------------------------------

    private HackMain hack_main;
    private HackTap hack_tap;
    private GameObject check_img;

    List<GameObject> ChildList = new List<GameObject>();
    List<GameObject> damyList = new List<GameObject>();


    private void Awake()
    {
        try
        {
            hack_main = GameObject.Find("controll").GetComponent<HackMain>();
            hack_tap = GameObject.Find("controll").GetComponent<HackTap>();
            GetWord = GameObject.Find("Canvas/Check/GetWord");
            CollectedWord = GameObject.Find("Canvas/PC/PassWordFase/Collect");
            CollectedPrefab = Resources.Load("Prefabs/MiniGame/Hacking/str") as GameObject;
            GetWordPrefab = Resources.Load("Prefabs/MiniGame/Hacking/WordImage") as GameObject;
            GetWordFolderPrefab = Resources.Load("Prefabs/MiniGame/Hacking/folder_str") as GameObject;
            CollectFolderPrefab = Resources.Load("Prefabs/MiniGame/Hacking/folder_str") as GameObject;
        }
        catch
        {
            Debug.Log("Not Find");
        }
        getNum.Clear();
        getDrawerNum.Clear();
    }

    /// <summary>
    /// 棚ver 単語のタップした時の処理
    /// </summary>
    /// <param name="placeNum"></param>
    public void SearchTap(int placeNum)
    {
        if (!getNum.Contains(placeNum))
        {
            getNum.Add(placeNum);

            //押したらアニメーション
            SoundManager.Instance.PlaySE(SEName.FindInfo);
            Text appearChild_text = gameObject.transform.GetChild(0).GetComponent<Text>();
            GetWordAnim(gameObject);
            DOTween.ToAlpha(
                () => appearChild_text.color,
                color => appearChild_text.color = color,
                0f, 2.0f);

            //PC内に集めた単語を表示
            GameObject _collected_word = Instantiate(CollectedPrefab, CollectedWord.transform);
            _collected_word.transform.position = hack_tap.pos_list[placeNum].transform.position;
            _collected_word.GetComponentInChildren<Text>().text = hack_main.Answer_list[placeNum].ToString();

            //集めたものリストの中に単語を表示
            GameObject _get_word = Instantiate(GetWordPrefab, GetWord.transform);
            _get_word.transform.SetAsFirstSibling();
            _get_word.GetComponentInChildren<Text>().text = hack_main.Answer_list[placeNum].ToString();
        }
        else
            return;
        
    }

    /// <summary>
    /// Drawer ver 単語のタップした時の処理
    /// </summary>
    /// <param name="place"></param>
    public void DrawerTap(int place)
    {
        string[] word = new string[3];
        word[0] = "ゲ";
        word[1] = "ー";
        word[2] = "ム";
        if (!getDrawerNum.Contains(place))
        {
            getDrawerNum.Add(place);
            GetWordAnim(gameObject);
            SoundManager.Instance.PlaySE(SEName.FindInfo);
            Text appearChild_text = gameObject.transform.GetChild(0).GetComponent<Text>();
            DOTween.ToAlpha(
                () => appearChild_text.color,
                color => appearChild_text.color = color,
                0f, 2.0f);

            //PC内に集めた単語を表示
            GameObject _collected_word = Instantiate(CollectFolderPrefab, hack_tap.CollectWordFolder.transform);
            _collected_word.transform.position = hack_tap.folder_pos_list[place].transform.position;
            _collected_word.GetComponentInChildren<Text>().text = word[place].ToString();

            //集めたものリストの中に単語を表示
            GameObject _get_word = Instantiate(GetWordFolderPrefab, GetWord.transform);
            _get_word.transform.SetAsFirstSibling();
            _get_word.GetComponentInChildren<Text>().text = word[place].ToString();
        }
        else
            return;
    }

    /// <summary>
    /// 文字取得時のDOToweenアニメーション処理
    /// </summary>
    /// <param name="obj">動かすオブジェクト</param>
    public void GetWordAnim(GameObject obj)
    {
        check_img = GameObject.Find("Canvas/Check/Image");
        Sequence seq = DOTween.Sequence();
        Image obj_img = obj.GetComponent<Image>();
        RectTransform obj_rect = obj.GetComponent<RectTransform>();
        seq.Append(obj_rect.DOMove(check_img.transform.position, 1.3f).SetEase(Ease.Linear))
            .Join(obj_rect.DOScale(new Vector2(0.5f, 0.5f), 1.3f))
            .Join((
                DOTween.ToAlpha(
                () => obj_img.color,
                color => obj_img.color = color,
                0f, 1.6f)));
    }

    /// <summary>
    /// ヒントを取得したらダミーで置いていた画像を消す処理
    /// </summary>
    public void GetDamy(int place)
    {
        GameObject parent = GameObject.Find("Canvas/Zoom");
        GameObject damy_parent = parent.transform.GetChild(place).gameObject;
        ChildList.Clear();
        damyList.Clear();
        for (int i = 0; i < damy_parent.transform.childCount; i++)
        {
            ChildList.Add(damy_parent.transform.GetChild(i).gameObject);
        }
        for (int j = 0; j < ChildList.Count; j++)
        {
            //  1層目のチェック
            if (ChildList[j].tag == "Damy")
            {
                damyList.Add(ChildList[j]);
            }
            //  2層目のチェックと
            else if (ChildList[j].transform.childCount == 1)
            {
                if (ChildList[j].transform.GetChild(0).tag == "Damy")
                {
                    damyList.Add(ChildList[j].transform.GetChild(0).gameObject);
                }
            }
        }
        foreach (var obj in damyList)
        {
            obj.SetActive(false);
        }
    }
}
