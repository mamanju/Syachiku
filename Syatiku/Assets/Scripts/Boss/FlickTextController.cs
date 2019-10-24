using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlickTextController : MonoBehaviour
{
    Text text;
    float alpha;
    //移動速度
    Vector3 moveForce;
    //反射中か
    bool isReflect;
    Color wrongColor = new Color(33 / 255f, 100 / 255f, 150 / 255f, 0);
    Color correctColor = new Color(240 / 255f, 179 / 255f, 37 / 255f, 0);

    enum Type
    {
        Wrong,
        Correct,
    }
    Type type;

    void Start()
    {
        text = GetComponent<Text>();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(int num, string t)
    {
        if(text == null) text = GetComponent<Text>();

        //座標
        float posX = Random.Range(0, 2);
        posX = (posX > 0 ? 450 : -450);
        float posY = Random.Range(-150, 150);
        Vector3 pos = new Vector3(posX, posY, 0);
        text.rectTransform.localPosition = pos;
        //移動
        float moveX = posX / Random.Range(100, 800);
        float moveY = Random.Range(-0.5f, 0.5f);
        moveForce = new Vector3(moveX, moveY, 0);

        //テキスト
        text.fontSize = Random.Range(100, 200);
        alpha = 0;
        this.type = (Type)num;
        text.color = (type == Type.Correct ? correctColor : wrongColor);
        text.text = t;

        isReflect = false;
        gameObject.SetActive(true);
    }

    void Update()
    {
        //透明度の更新
        if (alpha >= 1)
        {
            alpha = 1.0f;
        }
        else
        {
            alpha += 0.005f;
            Color c = text.color;
            c.a = alpha;
            text.color = c;
        }

        //移動
        text.rectTransform.localPosition += moveForce;

        //画面外に外れた時
        if (text.rectTransform.localPosition.x > 1400 || text.rectTransform.localPosition.x < -1400
            || text.rectTransform.localPosition.y > 700 || text.rectTransform.localPosition.y < -700)
        {
            gameObject.SetActive(false);
        }
    }

    public void FlickEnd()
    {
        if(!isReflect) BossScene.Instance.SetMoveForce(ref moveForce);
    }

    /// <summary>
    /// ボスとの衝突判定
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == Type.Correct)
        {
            SoundManager.Instance.PlaySE(SEName.CorrectHit);
            BossScene.Instance.SuccessCountUP();
            gameObject.SetActive(false);
        }
        else
        {
            SoundManager.Instance.PlaySE(SEName.WrongHit);
            BossScene.Instance.MissCountUP();
            isReflect = true;
            moveForce = new Vector3(-moveForce.x, moveForce.y, 0);
        }
    }
}
