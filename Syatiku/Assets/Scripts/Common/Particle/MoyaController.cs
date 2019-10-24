using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoyaController : MonoBehaviour {
    RectTransform parentRectTransform;

	void Start () {
        //parentRectTransform = transform.parent.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {

        
        //UI座標からスクリーン座標に変換
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, parentRectTransform.position);

        //ワールド座標
        Vector3 result = Vector3.zero;

        //スクリーン座標→ワールド座標に変換
        RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRectTransform, screenPos, Camera.main, out result);

        transform.position = result;
    }
}
