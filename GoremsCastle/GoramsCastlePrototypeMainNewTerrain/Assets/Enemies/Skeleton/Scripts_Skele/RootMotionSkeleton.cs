using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RootMotionSkeleton : MonoBehaviour, IDamageable
{
    private LayerMask layerMask;
    private NavMeshAgent Agent;
    private Animator Animator;

    private Vector2 Velocity;
    private Vector2 smoothDeltaPosition;

    private bool shouldAttack;
    private bool shouldMove;

    public int health = 100;

    public AttackRadius attackRadius;
    private Coroutine lookCoroutine;
    
    void Awake() {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();

        Animator.applyRootMotion = true;

        Agent.updatePosition = false;
        Agent.updateRotation = true;

        attackRadius.onAttack += onAttack;
        attackRadius.onExit += onExit;
    }
    // Update is called once per frame
    void Update()
    {
        SynchonizeAnimatorAndAgent();
    }

    private void onAttack(IDamageable gate)
    {
        shouldAttack = true;

       
    }

    private void onExit()
    {
        shouldAttack = false;
    }

    private void OnAnimatorMove() {
        Vector3 rootPosition = Animator.rootPosition;
        rootPosition.y = Agent.nextPosition.y;

        transform.position = rootPosition;
        Agent.nextPosition = rootPosition;
    }
    
    private void SynchonizeAnimatorAndAgent()
    {
        Vector3 worldDeltaPosition = Agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;

        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        Velocity = smoothDeltaPosition / Time.deltaTime;
        if (Agent.remainingDistance <= Agent.stoppingDistance)
        {
            Velocity = Vector2.Lerp(
                Vector2.zero,
                Velocity,
                Agent.remainingDistance / Agent.stoppingDistance
            );
        }
        shouldMove = Velocity.magnitude > 0.5f
        && Agent.remainingDistance > Agent.stoppingDistance 
        && shouldAttack == false;

        Animator.SetBool("Move", shouldMove);
        Animator.SetFloat("Blend", Velocity.magnitude);
        Animator.SetBool("Attack", shouldAttack);

        float deltaMagnitude = worldDeltaPosition.magnitude;
        if (deltaMagnitude > Agent.radius / 2f)
        {
            transform.position = Vector3.Lerp(
                Animator.rootPosition,
                Agent.nextPosition,
                smooth
            );
        }
    }
// This is the old attacking method, remove comments if new one sucks
    // void OnTriggerEnter(Collider other) {
    //     Debug.Log("yoho");
    //     if (other.gameObject.CompareTag("Gate"))
    //     {
    //         Animator.applyRootMotion = false;
    //         shouldAttack = true;
    //     }
    // }

    // private void OnTriggerExit(Collider other) {
    //     Debug.Log("yoho");
    //     if (other.gameObject.CompareTag("Gate"))
    //     {
    //         shouldAttack = false;
    //         Animator.applyRootMotion = true;
    //     }
    // }

    public virtual void SetUpAgentConfig()
    {
        Agent.acceleration = 8;
        Agent.angularSpeed =  120;
        Agent.areaMask = -1;
        Agent.avoidancePriority = 50;
        Agent.baseOffset = 0;
        Agent.height = 2f;
        Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        Agent.radius = 0.5f;
        Agent.speed = 3f;
        Agent.stoppingDistance=0.5f;

        health = 100;
        attackRadius.Collider.radius = 1.5f;
        attackRadius.delay =  1f;
        attackRadius.damageDone = 5;

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
