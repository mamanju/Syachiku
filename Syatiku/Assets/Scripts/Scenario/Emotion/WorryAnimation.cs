using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorryAnimation : MonoBehaviour {

    void Start()
    {
        StartCoroutine(StartAnimation());
        SoundManager.Instance.PlaySE(SEName.Worry);
    }

    IEnumerator StartAnimation()
    {
        Vector3 turn = new Vector3(1, -1, 1);
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.15f);
            transform.localScale = turn;
            yield return new WaitForSeconds(0.15f);
            transform.localScale = Vector3.one;
        }
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
