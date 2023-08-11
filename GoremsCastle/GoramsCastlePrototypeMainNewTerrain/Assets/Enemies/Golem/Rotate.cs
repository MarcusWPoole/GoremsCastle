using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float turnspeed = 0.1f;
    [SerializeField] Transform head;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FireRay();
    }
    void FireRay()
    {
        Ray ray = new Ray(head.position, head.forward);
        Debug.DrawRay(ray.origin, ray.direction * 10);
        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData))
        {
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.Cross(transform.right, hitData.normal));

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnspeed);

        
        }
    }
}
