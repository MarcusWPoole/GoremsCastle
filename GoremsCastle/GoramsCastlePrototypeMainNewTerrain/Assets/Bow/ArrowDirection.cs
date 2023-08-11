using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDirection : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Transform _transform;
    [SerializeField] private float flyingSpeed = 10.0f;
    //private List<Renderer> arrowRenderers = new List<Renderer>();
    private Renderer arrowRenderer;
    [SerializeField] public AudioSource arrowSound;
    private float fadeSpeed = 2f;
    public bool airborne = false;

    [SerializeField] public int damageValue;

    void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = transform;

        // arrowRenderers.Add(GetComponent<Renderer>());

        arrowRenderer = GetComponent<Renderer>();

        // for (int i = 0; i < _transform.childCount; i++)
        // {
        //     if (_transform.GetChild(i).gameObject.GetComponent<Renderer>() != null)
        //     {
        //         arrowRenderers.Add(_transform.GetChild(i).gameObject.GetComponent<Renderer>());
        //     }
        // }

        StartCoroutine(FadeInSingleRenderer());
    }

    void Update()
    {
        // Turn arrow to point in its direction of travel
        if (airborne)
        {
            //transform.forward = Vector3.Lerp(transform.forward, _rigidbody.velocity.normalized, Time.deltaTime * 10.0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_rigidbody.velocity.normalized), 1.0f);
        }
    }
    // IEnumerator FadeIn()
    // {
    //     float alpha = 0.0f;
    //     while (alpha < 255.0f)
    //     {
    //         Debug.Log("wtf");
    //         foreach (Renderer renderer in arrowRenderers)
    //         {
    //             renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alpha);
    //         }
    //         alpha += Time.deltaTime * fadeSpeed;
    //         yield return null;
    //     }
    // }

    // IEnumerator FadeInSingleRenderer()
    // {
    //     float alpha = 0.0f;
    //     while (alpha < 255.0f)
    //     {
    //         arrowRenderer.material.color = new Color(arrowRenderer.material.color.r, arrowRenderer.material.color.g, arrowRenderer.material.color.b, alpha);
        
    //         alpha += Time.deltaTime * fadeSpeed;
    //         yield return null;
    //     }
    // }

    IEnumerator FadeInSingleRenderer()
    {
        // Fade in arrow with axial wipe and noise
        float alpha = 0.0f;
        while (alpha < 1.2f)
        {
            arrowRenderer.material.SetFloat("_Fill", alpha);
            arrowRenderer.material.SetFloat("_NoiseFill", alpha);
        
            alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    IEnumerator FadeOutSingleRenderer()
    {
        // Fade out arrow with noise
        float alpha = 1.2f;
        while (alpha > -0.2f)
        {
            
            
            
            yield return null;
        }
    }

    IEnumerator FadeAndWaitForParticles()
    {
        // Wait for all particles to disappear before deleting arrow gameObject
        var particleCount = GetComponent<ParticleSystem>().particleCount;
        float alpha = 1.2f;
        while (particleCount > 0 && alpha > -0.2f)
        {
            arrowRenderer.material.SetFloat("_NoiseFill", alpha);
            alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Enemy")
        {
            airborne = false;
            _rigidbody.isKinematic = true;
            arrowSound.Play();
            
            //_transform.parent = collider.transform;
            //var emission = GetComponent<ParticleSystem>().emission
            collider.gameObject.GetComponent<IDamageable>().TakesDamage(damageValue);

            Destroy(gameObject);
            //emission.enabled = false;
            //StartCoroutine(FadeOutSingleRenderer());
            //StartCoroutine(FadeAndWaitForParticles());
        }
        else if (collider.tag == "Terrain" || collider.tag == "Path")
        {
            Destroy(gameObject);
        }
    }
}
