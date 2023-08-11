using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StringControl : MonoBehaviour
{
    private bool stringHeld = false;
    private bool bowHeld = false;

    private Vector3 defaultLocalPosition;
    private float stringExtent = 0.0f;

    private GameObject notchedArrow = null;
    private Rigidbody notchedArrowRigidbody = null;
    
    [SerializeField] private GameObject bow;
    [SerializeField] private GameObject bowString;
    [SerializeField] private GameObject arrowPrefab;

    [SerializeField] private float xDetachDistance = 10.0f;
    [SerializeField] private float zDetachDistance = 10.0f;

    private Transform _transform;
    private Transform bowTransform;

    private XRGrabInteractable xrGrabInteractable;
    [SerializeField] private AudioSource bowStringAudio;
    [SerializeField] private AudioSource bowReleaseAudio;

    private SkinnedMeshRenderer bowRenderer;
    private SkinnedMeshRenderer stringRenderer;

    [SerializeField] private Transform leftHandModel;
    [SerializeField] private Transform rightHandModel;
    [SerializeField] private Transform leftHandController;
    [SerializeField] private Transform rightHandController;
    private Transform pullingHand = null;
    private Transform pullingController = null;

    private Vector3 defaultHandRotation;
    private Vector3 defaultHandPosition;

    [SerializeField] private List<Renderer> glowObjects;

    // Start is called before the first frame update
    void Start()
    {
        _transform = transform;
        bowTransform = bow.GetComponent<Transform>();
        xrGrabInteractable = GetComponent<XRGrabInteractable>();

        defaultLocalPosition = bowTransform.InverseTransformPoint(_transform.position);

        bowRenderer = bow.GetComponent<SkinnedMeshRenderer>();
        stringRenderer = bowString.GetComponent<SkinnedMeshRenderer>();

        //xrGrabInteractable.OnSelectEntering += test;
    }

    // Update is called once per frame
    void Update()
    {
        if (stringHeld)
        {
            stringExtent = 100.0f * Mathf.Abs(bowTransform.InverseTransformPoint(_transform.position).y - defaultLocalPosition.y) / (0.006f - 0.00181f);
            bowRenderer.SetBlendShapeWeight(0, stringExtent);
            stringRenderer.SetBlendShapeWeight(0, stringExtent);

            foreach (Renderer renderer in glowObjects)
            {
                renderer.material.SetFloat("_NoiseFill", Mathf.Clamp(stringExtent/100f, -0.2f, 1.2f));
            }

            // Force let go of the string if hand moves too far from the bow
            if ((Mathf.Abs(bowTransform.InverseTransformPoint(_transform.position).x) > xDetachDistance | Mathf.Abs(bowTransform.InverseTransformPoint(_transform.position).z) > zDetachDistance) && xrGrabInteractable.enabled == true)
            {
                xrGrabInteractable.enabled = false;
            }

        }
        else if (xrGrabInteractable.enabled == false)
        {
            xrGrabInteractable.enabled = true;
        }

        // Set position of arrow to match the position of the hand
        if (notchedArrow != null)
        {
            //notchedArrow.transform.LookAt(bow.transform);
            //transform.LookAt(bow.transform);
            //notchedArrow.transform.Translate(Vector3 translation, Space relativeTo = Space.Self)
            notchedArrow.transform.localPosition = new Vector3(notchedArrow.transform.localPosition.x, bowTransform.InverseTransformPoint(_transform.position).y, notchedArrow.transform.localPosition.z);
        }

        //transform.InverseTransformPoint(_transform.position).y;

        //stringExtent = 100.0f * Mathf.Abs(_transform.localPosition.y - defaultLocalPosition.y) / (0.006f - 0.00181f);

    }

    public void OnSelectEnter()
    {
        stringHeld = true;

        if (bowHeld)
        {
            notchedArrow = Instantiate(arrowPrefab, bowTransform, false);
            //notchedArrow = Instantiate(arrowPrefab, new Vector3(defaultLocalPosition.x, bowTransform.InverseTransformPoint(_transform.position).y, defaultLocalPosition.z);
            notchedArrow.transform.localPosition = new Vector3(0f, defaultLocalPosition.y, 0f);
            notchedArrow.transform.localScale *= 0.015f;
            //Debug.Log(notchedArrow);
            //notchedArrow.transform.parent = _transform;
            //notchedArrow.transform.LookAt(bow.transform);
            notchedArrow.transform.rotation = bowTransform.rotation;
            notchedArrow.transform.Rotate(90f,0f,10f);

            bowStringAudio.Play();
            
            //notchedArrow.transform.parent = bowTransform;
            notchedArrowRigidbody = notchedArrow.GetComponent<Rigidbody>();

            if (pullingHand == null)
                {
                    if (Vector3.Distance(leftHandModel.position, _transform.position) < Vector3.Distance(rightHandModel.position, _transform.position))
                    {
                        pullingHand = leftHandModel;
                        pullingController = leftHandController;
                    }
                    else
                    {
                        pullingHand = rightHandModel;
                        pullingController = rightHandController;
                    }

                    defaultHandPosition = pullingHand.localPosition;
                    defaultHandRotation = pullingHand.localEulerAngles;
                    pullingHand.parent = notchedArrow.transform;
                }
        }
    
    }

    public void OnSelectExit()
    {
        // if (notchedArrow != null)
        // {
        //     _transform.parent = bowTransform

        //     notchedArrow = null;
        //     notchedArrowRigidbody = null;
        // }
        if (notchedArrow != null)
        {
            notchedArrow.transform.parent = null;
            notchedArrowRigidbody.isKinematic = false;
            notchedArrowRigidbody.AddForce(10.0f * stringExtent * notchedArrow.transform.forward);
            var emission = notchedArrow.GetComponent<ParticleSystem>().emission;
            emission.enabled = true;
            notchedArrow.GetComponent<ArrowDirection>().airborne = true;

            bowReleaseAudio.Play();

            notchedArrowRigidbody = null;
            notchedArrow = null;

            pullingHand.parent = pullingController;
            pullingHand.localPosition = defaultHandPosition;
            pullingHand.localEulerAngles = defaultHandRotation;

            pullingHand = null;
            pullingController = null;
        }


        stringHeld = false;
        _transform.localPosition = defaultLocalPosition;
        
        

        stringExtent = 0.0f;
        bowRenderer.SetBlendShapeWeight(0, stringExtent);
        stringRenderer.SetBlendShapeWeight(0, stringExtent);

        foreach (Renderer renderer in glowObjects)
            {
                renderer.material.SetFloat("_NoiseFill", -0.2f);
            }
    }

    public void OnBowGrabbed()
    {
        bowHeld = true;
    }

    public void OnBowReleased()
    {
        bowHeld = false;
    }

    // void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.tag == "Player")
    //     {
    //         Debug.Log("Check");
    //         stringHeld = true;
    //         notchedArrow = collision.gameObject;
    //         notchedArrowRigidbody = collision.rigidbody;
    //         _transform.parent = collision.transform;
    //     }
    // }
}
