using System.Collections;
using UnityEngine;

public class JoyAnimation : MonoBehaviour {

	void Start () {
       StartCoroutine(StartAnimation());
    }

    IEnumerator StartAnimation()
    {
        bool isJoy = gameObject.name.Contains("Joy");
        float time = 0;
        int max = 0;
        if (isJoy)
        {
            time = 0.3f;
            max = 3;
            SoundManager.Instance.PlaySE(SEName.Joy);
        }
        else
        {
            time = 0.1f;
            max = 5;
            //SoundManager.Instance.PlaySE(SEName.);
        }
        Vector3 turn = new Vector3(-1, 1, 1);
        for (int i = 0; i < max; i++)
        {
            yield return new WaitForSeconds(time);
            transform.localScale = turn;
            yield return new WaitForSeconds(time);
            transform.localScale = Vector3.one;
        }
        yield return new WaitForSeconds(time);
        Destroy(gameObject);


    }
}
