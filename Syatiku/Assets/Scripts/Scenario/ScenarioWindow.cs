using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class ScenarioWindow : MonoBehaviour
{
    [System.SerializableAttribute]
    public class ListWrapper
    {
        public List<Vector3> list = new List<Vector3>();
        public ListWrapper(List<Vector3> l) { list = l; }
    }

    public Vector3 closeMenuPos;
    public Vector3 opneMenuPos;
    public Image bgi;
    public Text name;
    public Text message;
    public Text logText;
    public GameObject log;
    //0:喜 1:怒 3:哀 4:溜息 5:？ 6:悩 7:焦 8:決
    public GameObject[] emotionPrefabs;
    //0:左 1:中央 2:右
    public List<ListWrapper> emotionPosList = new List<ListWrapper>();
    public Transform emotionsParent;
    public GameObject recommendLight;
    public Image[] characters;
    //メニュー
    public Sprite[] menuSprites;
    public RectTransform menu;
    public Image menuButton;
    public Image skipButton;
    public Image autoButton;
    public Image logButton;
    public ScrollRect scroll;

    public CanvasGroup scenarioCanvas;

    void Start()
    {
        //バグの強制回避
        closeMenuPos = new Vector3(-300, 180, 0);
        menu.localPosition = closeMenuPos;
        //これだけでいいはずなのに...
        //closeMenuPos = menu.localPosition;
    }

    public void Init()
    {
        //メニュー
        menu.localPosition = closeMenuPos;
        menu.gameObject.SetActive(true);
        menuButton.sprite = menuSprites[0];
        //キャラ
        for (int i = 0;i < characters.Length; i++)
        {
            characters[i].gameObject.SetActive(false);
        }
        recommendLight.SetActive(false);
        //テキスト
        logText.text = "";
        name.text = "";
        message.text = "";
        //キャンバス
        scenarioCanvas.alpha = 0.01f;
        gameObject.SetActive(false);

        bgi.sprite = null;
    }
}