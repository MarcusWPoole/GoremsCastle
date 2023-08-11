using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AttackRadius : MonoBehaviour
{    

    private List<IDamageable> damageables = new List<IDamageable>();
    public SphereCollider Collider;
    public int damageDone = 10;
    public float delay = 0.5f;
    public delegate void AttackEvent(IDamageable Gate);
    public AttackEvent onAttack;

    public delegate void ExitEvent();
    public ExitEvent onExit;
    private Coroutine attackCoroutine;

    void Awake()
    {
        Collider = GetComponent<SphereCollider>();
    }
    
    private void OnTriggerEnter(Collider other) {
        
        if (other.CompareTag("Gate"))
        {
            Debug.Log("YOYOYO");
            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
            {
                Debug.Log("damageable found");
                damageables.Add(damageable);
                if(attackCoroutine == null)
                {
                    attackCoroutine = StartCoroutine(Attack(other.GetComponent<IDamageable>()));
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {

        Debug.Log("collider exit");
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable != null)
        {
            damageables.Remove(damageable);
            Debug.Log("Damagables amount: " + damageables.Count);
            if(damageables.Count == 0)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
            Debug.Log("yoyo");
            onExit?.Invoke();
        }
        
    }

    private IEnumerator Attack(IDamageable gate)
    {
        WaitForSeconds wait = new WaitForSeconds(delay);

        yield return wait;
        while(damageables.Count > 0)
        {
            onAttack?.Invoke(gate);

            gate.TakesDamage(damageDone);

            yield return wait;

            damageables.RemoveAll(disabledDamagables);
        }
    
        //attackCoroutine = null;
        yield break;
    }

    private bool disabledDamagables(IDamageable disabled)
    {
        return disabled != null && !disabled.GetTransform().gameObject.activeSelf;
    }

}
