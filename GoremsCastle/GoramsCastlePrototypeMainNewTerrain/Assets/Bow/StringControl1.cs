using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StringControl1 : MonoBehaviour
{
    private bool stringHeld = false;

    private Vector3 defaultLocalPosition;
    private float stringExtent = 0.0f;

    private GameObject notchedArrow = null;
    private Rigidbody notchedArrowRigidbody = null;
    
    [SerializeField] private GameObject bow;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform upperAttach;
    [SerializeField] private Transform lowerAttach;
    //[SerializeField] private LineRenderer lineRenderer;

    private Vector3 UpperRelaxedVector = new Vector3(0.0f, 0.00176f, 0.0034f);
    private Vector3 UpperStretchedVector = new Vector3(0.0f, 0.00265f, 0.00328f);
    private Vector3 LowerRelaxedVector = new Vector3(0.0f, 0.00176f, -0.0034f);
    private Vector3 LowerStretchedVector = new Vector3(0.0f, 0.00265f, -0.00328f);

    [SerializeField] private float xDetachDistance = 10.0f;
    [SerializeField] private float zDetachDistance = 10.0f;

    private Transform _transform;
    private Transform bowTransform;

    private XRGrabInteractable xrGrabInteractable;

    private SkinnedMeshRenderer bowRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _transform = transform;
        bowTransform = bow.GetComponent<Transform>();
        xrGrabInteractable = GetComponent<XRGrabInteractable>();

        defaultLocalPosition = bowTransform.InverseTransformPoint(_transform.position);

        bowRenderer = bow.GetComponent<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stringHeld)
        {
            stringExtent = Mathf.Abs(bowTransform.InverseTransformPoint(_transform.position).y - defaultLocalPosition.y) / (0.006f - 0.00181f);

            upperAttach.position = Vector3.Lerp(UpperRelaxedVector, UpperStretchedVector, Mathf.Clamp(stringExtent, 0f, 1f));
            lowerAttach.position = Vector3.Lerp(LowerRelaxedVector, LowerStretchedVector, Mathf.Clamp(stringExtent, 0f, 1f));

            if ((Mathf.Abs(bowTransform.InverseTransformPoint(_transform.position).x) > xDetachDistance | Mathf.Abs(bowTransform.InverseTransformPoint(_transform.position).z) > zDetachDistance) && xrGrabInteractable.enabled == true)
            {
                xrGrabInteractable.enabled = false;
            }
        }
        else if (xrGrabInteractable.enabled == false)
        {
            xrGrabInteractable.enabled = true;
        }

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
        notchedArrow = Instantiate(arrowPrefab, bowTransform, false);
        //notchedArrow = Instantiate(arrowPrefab, new Vector3(defaultLocalPosition.x, bowTransform.InverseTransformPoint(_transform.position).y, defaultLocalPosition.z);
        notchedArrow.transform.localPosition = new Vector3(0f, defaultLocalPosition.y, 0f);
        notchedArrow.transform.localScale *= 0.015f;
        Debug.Log(notchedArrow);
        //notchedArrow.transform.parent = _transform;
        notchedArrow.transform.LookAt(bow.transform);
        //notchedArrow.transform.parent = bowTransform;
        notchedArrowRigidbody = notchedArrow.GetComponent<Rigidbody>();
    }

    public void OnSelectExit()
    {
        // if (notchedArrow != null)
        // {
        //     _transform.parent = bowTransform

        //     notchedArrow = null;
        //     notchedArrowRigidbody = null;
        // }
        notchedArrow.transform.parent = null;
        notchedArrowRigidbody.isKinematic = false;
        notchedArrowRigidbody.AddForce(10.0f * stringExtent * notchedArrow.transform.forward);

        notchedArrowRigidbody = null;
        notchedArrow = null;

        stringHeld = false;
        _transform.localPosition = defaultLocalPosition;

        stringExtent = 0.0f;
        bowRenderer.SetBlendShapeWeight(0, stringExtent);
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
