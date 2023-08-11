using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlacementBehaviour : MonoBehaviour
{
    
    private XRSocketInteractor tower_socket;

    private bool tower_active;
    
    // Start is called before the first frame update
    void Start()
    {
        tower_socket = gameObject.GetComponent<XRSocketInteractor>();
        tower_active = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnSelectEnter()
    {
        Debug.Log("here");
    }
}
