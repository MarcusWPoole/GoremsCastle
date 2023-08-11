using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCannon : MonoBehaviour
{
    [SerializeField] private GameObject cannonballPrefab;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private SkinnedMeshRenderer barrelRenderer;
    [SerializeField] private Renderer chargeRenderer;
    [SerializeField] private HingeJoint lever;

    private float fadeSpeed = 2f;

    public bool ready = false;
    private float chargeTime = 0.0f;
    [SerializeField] private float timeToFullyCharge = 10.0f;

    [SerializeField] private float minSpeed = 10.0f;
    [SerializeField] private float maxSpeed = 50.0f;
    private float minAngle = 45;
    private float maxAngle = -45f;
    private float speed;
    [SerializeField] private float barrelRate = 1f;
    private Transform _transform;
    private GameObject cannonball = null;
    [SerializeField] private float delT = 0.1f;
    [SerializeField] int iterations = 100;
    private Vector3 pos;
    private Vector3 vel;
    private Vector3 acc;
    private Vector3[] vertices;

    private AudioSource cannonSound;

    // Start is called before the first frame update
    void Start()
    {
        _transform = transform;
        acc = Physics.gravity;
        chargeRenderer.material.SetFloat("_Fill", 0f);
        chargeRenderer.material.SetFloat("_NoiseFill", 1.2f);

        cannonSound = GetComponent<AudioSource>();
        //StartCoroutine(FadeOutSingleRenderer());
    }

    // Update is called once per frame
    void Update()
    {
        if (chargeTime < timeToFullyCharge)
        {
            chargeTime += Time.deltaTime;

            float fillFraction = (chargeTime/timeToFullyCharge) * 0.19f;

            chargeRenderer.material.SetFloat("_Fill", fillFraction);
        }
        else if (!ready)
        {
            ready = true;
            StartCoroutine(FadeInSingleRenderer());
        }

        // if (lever.angle < 180)
        // {
        speed = Remap(lever.angle);
        //Debug.Log(lever.angle);
        // }
        // else
        // {
        //     speed = Remap(lever.angle - 360f);
        //     Debug.Log(lever.angle - 360f);
        // }

        if (Input.GetKeyDown("space"))
        {
            Fire();
        }

        DrawTrajectory();
    }

    IEnumerator BarrelDistort()
    {
        float timer = 0.0f;
        while (timer < Mathf.PI)
        {
            barrelRenderer.SetBlendShapeWeight(0, 100f * Mathf.Sin(timer));
            timer += Time.deltaTime * barrelRate;
            yield return null;
        }
    }

    IEnumerator FadeInSingleRenderer()
    {
        // Fade in charge with noise
        float alpha = 0.0f;
        while (alpha < 1.2f)
        {
            chargeRenderer.material.SetFloat("_NoiseFill", alpha);
        
            alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    IEnumerator FadeOutSingleRenderer()
    {
        // Fade in charge with noise
        float alpha = 1.2f;
        while (alpha > -0.2f)
        {
            chargeRenderer.material.SetFloat("_NoiseFill", alpha);
            
            alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    public void Fire()
    {
        if (ready)
        {
            StartCoroutine(BarrelDistort());
            cannonball = Instantiate(cannonballPrefab, spawnPoint);
            cannonball.transform.parent = null;
            //cannonball.transform.localScale /= _transform.localScale.magnitude;
            cannonball.GetComponent<Rigidbody>().AddForce(-speed * _transform.right, ForceMode.VelocityChange);
            cannonSound.Play();
            cannonball = null;
            ready = false;
            chargeTime = 0.0f;
            //StartCoroutine(FadeOutSingleRenderer());
            chargeRenderer.material.SetFloat("_Fill", 0f);
            chargeRenderer.material.SetFloat("_NoiseFill", 1.2f);
        }

    }

    private float Remap(float angle)
    {
        return (minSpeed - maxSpeed)/(minAngle - maxAngle) * angle + minSpeed - minAngle * (minSpeed - maxSpeed)/(minAngle - maxAngle);
    }

    private void DrawTrajectory()
    {
        pos = spawnPoint.position;
        vel = -speed * _transform.right;
        vertices = new Vector3[iterations];

        for (int i = 0; i < iterations; i++)
        {
            vertices[i] = pos;
            vel += delT * acc;
            pos += delT * vel;
        }

        lineRenderer.positionCount = iterations;
        lineRenderer.SetPositions(vertices);
    }
}
