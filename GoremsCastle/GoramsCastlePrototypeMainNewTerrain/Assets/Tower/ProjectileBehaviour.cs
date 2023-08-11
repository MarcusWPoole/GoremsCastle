using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script handles the behaviour of the fireball that each tower creates.

public class ProjectileBehaviour : MonoBehaviour
{
    
    private Transform target;

    public float speed = 70f;
    public int damage = 50;
 
    public GameObject impactEffect;
    public AudioSource projectileSound;

    public void Seek(Transform targetSeek)
    {
        target = targetSeek;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = target.position - transform.position;
        float distanceFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceFrame, Space.World);
    }

    void HitTarget()
    {
        AudioSource.PlayClipAtPoint(projectileSound.clip, target.position);
        
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 2f);
        
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.gameObject.GetComponent<IDamageable>().TakesDamage(damage);
        }
    }


}
