using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] List<GameObject> bodyParts;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {   
        if (animator != null)
        {
             animator.SetBool("Clonk", Input.GetKey(KeyCode.Space));

            if (Input.GetKeyDown("a"))
            {
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
       
    }


}
