using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTester : MonoBehaviour
{
    [SerializeField]
    private PositionSystem _positionSystem;

    [SerializeField]
    private TeleportMarker _positionA;
    [SerializeField]
    private TeleportMarker _positionB;
    [SerializeField]
    private TeleportMarker _positionC;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("q"))
        {
            //_positionSystem.TeleportToMarker(gameObject, _positionA);
        }
        if(Input.GetKeyDown("w"))
        {
            //_positionSystem.TeleportToMarker(gameObject, _positionB);
        }
        if(Input.GetKeyDown("e"))
        {
            //_positionSystem.TeleportToMarker(gameObject, _positionC);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = new Color(1f, 0f, 1f, 1f);
        Gizmos.DrawLine(Vector3.up*3, Vector3.up*3+Vector3.forward);
    }
}
