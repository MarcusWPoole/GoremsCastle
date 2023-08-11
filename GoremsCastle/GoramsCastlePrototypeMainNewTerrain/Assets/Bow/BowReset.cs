using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowReset : MonoBehaviour
{

    [SerializeField] private PositionSystem posSys;
    [SerializeField] private Transform origin;

    private Transform marker;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        marker = posSys._currentTeleportMarker.gameObject.transform;

        if (transform.position.z < (marker.position.z - 2f))
        {
            transform.position = origin.position;
        }

    }
}
