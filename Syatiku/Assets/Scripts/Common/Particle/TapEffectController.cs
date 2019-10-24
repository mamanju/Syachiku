
using UnityEngine;

public class TapEffectController : MonoBehaviour {
    [SerializeField]
    GameObject tapEffect;
	
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            if (!tapEffect.activeSelf)
            {
                Vector3 mousePos = Input.mousePosition;
                //スクリーン座標からワールド座標へ変換
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                //z軸修正
                worldPos.z = 0;
                tapEffect.transform.position = worldPos;
                tapEffect.SetActive(true);
            }
        }
	}
}
