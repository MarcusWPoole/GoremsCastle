using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour, IDamageable
{
    public Transform goal;
    private NavMeshAgent agent;

    public int health = 15;

    public List<Transform> waypoints;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(goal.position);
        }
    }

    public void TakesDamage(int Damage)
    {
        health -= Damage;

        if (health <=0)
        {
            // add skeleton death here
            gameObject.SetActive(false);
        }
    }

    public Transform GetTransform()
    {
        throw new System.NotImplementedException();
    }
}
