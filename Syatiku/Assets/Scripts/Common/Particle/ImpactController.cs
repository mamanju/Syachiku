using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactController : ParticleController {

    RectTransform rect;

    protected override void Awake()
    {
        rect = GetComponent<RectTransform>();
        base.Awake();
    }

    protected override void OnEnable()
    {
        int rnd = Random.Range(0, 2);
        float x = rnd > 0 ? Random.Range(100f, 200f) : Random.Range(-100f, -200f);
        rnd = Random.Range(0, 2);
        float y = rnd > 0 ? Random.Range(100f, 200f) : Random.Range(-100f, -200f);
        rect.localPosition = new Vector3(x, y, 0);
        base.OnEnable();
    }
}
