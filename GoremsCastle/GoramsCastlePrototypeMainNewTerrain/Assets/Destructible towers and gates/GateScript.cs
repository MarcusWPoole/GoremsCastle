using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateScript : MonoBehaviour, IDamageable
{
    [SerializeField]
    public int health = 300;

    private MeshFilter _meshfilter;
    [SerializeField] Mesh damageMesh1;
    [SerializeField] Mesh damageMesh2;
    [SerializeField] Mesh damageMesh3;

    private bool gateDestroyed;

    void Start()
    {
        _meshfilter = GetComponent<MeshFilter>();
        gateDestroyed = true;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void TakesDamage(int Damage)
    {
        health -= Damage;
        if (200 < health && health <= 250)
        {
            
            // change to make the gate crumbled.
            //gameObject.SetActive(false);
            _meshfilter.mesh = damageMesh1;

        }
        else if(100 < health && health <= 200)
        {
            _meshfilter.mesh = damageMesh2;
        }
        else if(50 < health && health <= 100)
        {
            _meshfilter.mesh = damageMesh3;
        }
        else if(health <= 0)
        {
            //gameObject.SetActive(false);

            transform.Translate(Vector3.down * 100f, Space.World);
            Debug.Log("Gate Destroyed");
        }
    }
}
