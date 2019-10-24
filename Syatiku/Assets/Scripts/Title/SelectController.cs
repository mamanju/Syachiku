using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectController : MonoBehaviour,
     IPointerEnterHandler, IPointerExitHandler {
    [SerializeField]
    private Image blackMan, whiteMan;
    [SerializeField]
    private float zoomScale;

    /// <summary>
    /// カーソルが置いてあるときの処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData) {
        switch (gameObject.name) {
            case "White":
                blackMan.transform.localScale = new Vector2(zoomScale, zoomScale);
                break;
            case "Another":
                whiteMan.transform.localScale = new Vector2(zoomScale, zoomScale);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// カーソルが置いてないときの処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData) {
        switch (gameObject.name) {
            case "White":
                blackMan.transform.localScale = new Vector2(1f, 1f);
                break;
            case "Another":
                whiteMan.transform.localScale = new Vector2(1, 1f);
                break;
            default:
                break;
        }
    }
}
