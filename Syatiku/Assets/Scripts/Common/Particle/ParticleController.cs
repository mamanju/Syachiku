using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour {

    ParticleSystem particle;

    protected virtual void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    protected virtual void OnEnable()
    {
        //ループしないパーティクル
        if (!particle.loop)
        {
            StartCoroutine(DisableParticle());
        }
    }

    /// <returns></returns>
    IEnumerator DisableParticle()
    {
        yield return new WaitWhile(() => particle.IsAlive(true));

        gameObject.SetActive(false);
    }
}
