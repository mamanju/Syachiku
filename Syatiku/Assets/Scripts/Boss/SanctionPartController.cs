using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SanctionPartController : MonoBehaviour {

    [SerializeField]
    Sprite[] slappedBossSprites;
    [SerializeField]
    RectTransform slappedBoss;
    GameObject slappedBossGO;
    [SerializeField]
    GameObject impactEffect;
    [SerializeField]
    RectTransform effectBox;
    List<GameObject> effectList = new List<GameObject>();

    [SerializeField]
    float endTime;
    float timer = 0;

    public void Initialize(int gameMode)
    {
        slappedBoss.GetComponent<UnityEngine.UI.Image>().sprite = slappedBossSprites[gameMode];
        slappedBossGO = slappedBoss.gameObject;
    }

    public void UpdateSanctionPart()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SoundManager.Instance.PlaySE(SEName.Harisen);
            if (!slappedBossGO.activeSelf)
            {
                BossScene.Instance.ChangeBossState(slappedBossGO);
            }
            BossScene.Instance.ChangeDamageGage(2);
            Vector3 bossScale = slappedBoss.localScale;
            bossScale.x *= -1;
            //向きの変更
            slappedBoss.localScale = bossScale;

            //エフェクトのオブジェクトプール
            for (int i = 0; i < effectList.Count; i++) {
                if (!effectList[i].activeSelf)
                {
                    effectList[i].SetActive(true);
                    return;
                }
            }
            GameObject effect = Instantiate(impactEffect, effectBox);
            effectList.Add(effect);
        }

        UpdateTimer();
    }

    private void UpdateTimer()
    {
        timer += Time.deltaTime;
        if (timer > endTime)
        {
            timer = 0;
            slappedBoss.gameObject.SetActive(false);
            for (int i = 0; i < effectList.Count; i++)
            {
                effectList[i].SetActive(false);
            }
            BossScene.Instance.ChangePart();
        }
    }
}
