using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveToGolem : MonoBehaviour, IDamageable
{
    public Transform goal;
    [SerializeField] float smoothing = 10f;
    [SerializeField] Transform head;
    private NavMeshAgent agent;

    [SerializeField] List<GameObject> bodyParts;

    [SerializeField] List<Renderer> bodyPartRenderers;

    [SerializeField] Material dmgTex1;
    [SerializeField] Material dmgTex2;
    [SerializeField] Material dmgTex3;

    private Animator animator;

    [SerializeField] float delay = 0.5f;
    private bool shouldAttack;
    private bool shouldMove;

    [SerializeField] int clonkDamage = 50;

    [SerializeField] public AudioSource golemDamageSound;

    public List<Transform> waypoints;

    private int healthMax;

    public int health = 500;

      private Coroutine clonkCoroutine;

   
    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        animator = GetComponentInChildren<Animator>();

        Debug.Log(animator.name);

        healthMax = health;

        

        SetUpAgentConfig();
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(goal.position);

         rotateGolem();

         SynchonizeAnimatorAndAgent();

        if (Input.GetKeyDown("space"))
        {
            TakesDamage(10);
        }
        Debug.Log(health.ToString());
    }

     private void OnTriggerEnter(Collider other) {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            if(clonkCoroutine == null)
            {
                 if(other.gameObject.CompareTag("Gate"))
                {
                    Debug.Log("collided with gate");
                    agent.updatePosition = false;
                    shouldAttack = true;
                    shouldMove = false;
                    clonkCoroutine = StartCoroutine(Clonk(other.GetComponent<IDamageable>()));
                 }
            }
        }
        clonkCoroutine = null;  
     }
    

    private void OnTriggerExit(Collider collider)
    {
        shouldAttack = false;
        shouldMove = true;
        agent.updatePosition = true;

        //StopCoroutine(clonkCoroutine);
        //clonkCoroutine = null;
    }

    void rotateGolem()
    {
     Ray ray = new Ray(head.position, head.forward);
        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData))
        {
            if (hitData.transform.CompareTag ("Path"))
             transform.rotation  = Quaternion.LookRotation(Vector3.Cross(transform.right, hitData.normal));
        }
    }

    public virtual void SetUpAgentConfig()
    {

    }
    private void SynchonizeAnimatorAndAgent()
    {
        if (agent.velocity.magnitude > 0.1f && shouldAttack == false)
        {
            shouldMove = true;
        }

        animator.SetBool("Walk", shouldMove);
        animator.SetBool("Clonk", shouldAttack);

    }

    public void TakesDamage(int Damage)
    {
        health -= Damage;

        golemDamageSound.Play();

        if (2*healthMax/3 < health && health <= healthMax)
        {
            foreach (Renderer renderer in bodyPartRenderers)
            {
                renderer.material = dmgTex1;
            }
        }
        else if (healthMax/3 < health && health <= 2*healthMax/3)
        {
            foreach (Renderer renderer in bodyPartRenderers)
            {
                renderer.material = dmgTex2;
            }
        }
        else if (0 < health && health <= healthMax/3)
        {
            foreach (Renderer renderer in bodyPartRenderers)
            {
                renderer.material = dmgTex3;
            }
        }
        else if (health <=0)
        {
            // add Golem destruction here
            foreach (GameObject bodyPart in bodyParts)
                {
                    bodyPart.AddComponent<Rigidbody>();
                    bodyPart.GetComponent<Rigidbody>().drag = 1;
                    bodyPart.GetComponent<Rigidbody>().angularDrag = 1;
                    bodyPart.AddComponent<CapsuleCollider>();
                    int count = bodyPart.transform.childCount;
                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            Destroy(bodyPart.transform.GetChild(i).gameObject);
                        }

                    }
                    bodyPart.transform.parent = null;
                    
                }

            Destroy(gameObject);
        }
    }

    public Transform GetTransform()
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator Clonk(IDamageable gate)
    {
        WaitForSeconds wait = new WaitForSeconds(delay);

        yield return wait;
        while(shouldAttack == true)
        {
            
            gate.TakesDamage(clonkDamage);
            yield return wait;
        }
    
        clonkCoroutine = null;
    }
}
