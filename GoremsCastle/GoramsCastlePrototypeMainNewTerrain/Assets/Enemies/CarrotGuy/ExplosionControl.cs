using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionControl : MonoBehaviour
{
    [SerializeField] private float emitLifetime = 1f;
    [SerializeField] private float objectLifetime = 10f;
    private ParticleSystem particles;
    

    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        var emitter = particles.emission;
        emitter.enabled = true;
        Invoke("EmitEnd", emitLifetime);
        Invoke("DeleteMe", objectLifetime);
    }

    void EmitEnd()
    {
        var emitter = particles.emission;
        emitter.enabled = false;
    }

    void DeleteMe()
    {
        Destroy(gameObject);
    }

}
