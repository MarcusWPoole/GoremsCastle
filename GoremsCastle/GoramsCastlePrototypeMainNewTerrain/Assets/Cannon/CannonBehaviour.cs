using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBehaviour : MonoBehaviour
{
    
    [SerializeField] public GameObject cannonHitEffect;
    private GameObject explosion;
    [SerializeField] public int cannonDamage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            explosion = Instantiate(cannonHitEffect, transform);
            explosion.transform.parent = null;
            other.gameObject.GetComponent<IDamageable>().TakesDamage(cannonDamage);
            Destroy(gameObject);
        }
        else if (GetComponent<Collider>().tag == "Terrain" || GetComponent<Collider>().tag == "Path")
        {
            explosion = Instantiate(cannonHitEffect, transform);
            explosion.transform.parent = null;
            Destroy(gameObject);
        }
        else
        {
            return;
        }
    }
}
