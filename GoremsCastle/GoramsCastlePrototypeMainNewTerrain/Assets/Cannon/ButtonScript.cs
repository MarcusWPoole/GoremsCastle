using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] private Transform buttonTransform;
    //private bool pressed = false;
    private Vector3 restPos = new Vector3(-0.000185950295f, 0f, 5.06461e-05f);
    private Vector3 pressedPos = new Vector3(-0.000186000005f, 0f, -9.00000032e-06f);
    private GameObject pressingObject = null;
    public UnityEvent buttonWasPressed;

    // Start is called before the first frame update
    void Start()
    {
        buttonTransform.localPosition = restPos;
    }

    void OnTriggerEnter(Collider collider)
    {
        pressingObject = collider.gameObject;
        buttonTransform.localPosition = pressedPos;
        buttonWasPressed.Invoke();
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject == pressingObject)
        {
            buttonTransform.localPosition = restPos;
            pressingObject = null;
        }
    }
}
