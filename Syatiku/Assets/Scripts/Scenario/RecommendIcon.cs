using UnityEngine;

public class RecommendIcon : MonoBehaviour {

    Vector3 downPos;
    Vector3 upPos;
    Vector3 targetPos;

    [SerializeField, Header("上下する速さ")]
    float speed = 10.0f;

    void Start()
    {
        downPos = transform.localPosition;
        upPos = new Vector3(downPos.x, downPos.y + 20, downPos.z);
        targetPos = upPos;
    }

    void Update () {
        if(Vector3.Distance(transform.localPosition, targetPos) < 0.5f)
        {
            targetPos = (targetPos == downPos ? upPos : downPos);
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * speed);
	}
}
