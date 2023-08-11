using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelieTurnyScript : MonoBehaviour
{
    [SerializeField] private Transform cannonFrameTransform;
    private Transform _transform;

    // Start is called before the first frame update
    void Start()
    {
        _transform = transform;        
    }

    // Update is called once per frame
    void Update()
    {
        _transform.localEulerAngles = new Vector3(_transform.localEulerAngles.x, cannonFrameTransform.localEulerAngles.y, _transform.localEulerAngles.z);
        //Debug.Log(cannonFrameTransform.eulerAngles.y.ToString());
    }
}