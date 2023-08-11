using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Hand : MonoBehaviour
{
    
    public float speed;

    Animator animator;
    SkinnedMeshRenderer mesh;
    private float grip_target;
    private float trigger_target;
    private float grip_current;
    private float trigger_current;
    private string grip_anim_param = "Grip";
    private string trigger_anim_param = "Trigger";

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimateHand();
    }

    internal void SetGrip(float degree)
    {
        grip_target = degree;
    }

    internal void SetTrigger(float degree)
    {
        trigger_target = degree;
    }

    void AnimateHand()
    {
        if (grip_current != grip_target)
        {
            Debug.Log("Grip Working");
            grip_current = Mathf.MoveTowards(grip_current, grip_target, Time.deltaTime * speed);
            animator.SetFloat(grip_anim_param, grip_current);
        }

        if (trigger_current != trigger_target)
        {
            Debug.Log("Trigger Working");
            trigger_current = Mathf.MoveTowards(trigger_current, trigger_target, Time.deltaTime * speed);
            animator.SetFloat(trigger_anim_param, trigger_current);
        }
    }

    public void ToggleVisibility()
    {
        mesh.enabled = !mesh.enabled;
    }
}
