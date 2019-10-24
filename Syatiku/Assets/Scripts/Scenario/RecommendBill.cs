using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecommendBill : MonoBehaviour {
    [SerializeField]
    private float distance;
    [SerializeField]
    private Sprite isLightImage, noLightImage;
	
    void OnEnable()
    {
        StartCoroutine(Lighting(distance));
    }

    public IEnumerator Lighting(float d)
    {
        while (true)
        {
            gameObject.GetComponent<Image>().sprite = noLightImage;
            yield return new WaitForSeconds(d);
            gameObject.GetComponent<Image>().sprite = isLightImage;
            yield return new WaitForSeconds(d);
        }
    }
}
