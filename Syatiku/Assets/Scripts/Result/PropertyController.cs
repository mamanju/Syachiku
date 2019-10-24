using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PropertyController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(FadeIn());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator FadeIn()
    {
        yield return null;
        while (GetComponent<Image>().color.a < 1)
        {
            GetComponent<Image>().color += new Color(0, 0, 0, Mathf.Lerp(GetComponent<Image>().color.a, 1, 0.1f));
            transform.GetChild(0).GetComponent<Text>().color += new Color(0, 0, 0, Mathf.Lerp(GetComponent<Image>().color.a,1,0.1f));
            yield return null;
        }
        transform.DOScale(new Vector3(1, 1, 1), 0.2f).SetEase(Ease.Linear);
    }
}
