using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotExplosion : MonoBehaviour
{
    [SerializeField] GameObject explosionEffect;
    GameObject instancedExplosion = null;

    private void OnTriggerEnter(Collider other) {
        
        if (other.CompareTag("Gate"))
        {
            Debug.Log("CarrotHit");
            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
            {
                instancedExplosion = Instantiate(explosionEffect, transform);
                instancedExplosion.transform.parent = null;
                damageable.TakesDamage(10);
                Destroy(gameObject);
            }
        }
    }
}
