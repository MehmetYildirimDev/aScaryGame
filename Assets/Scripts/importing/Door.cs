using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{

    private bool isOpen = false;
    private bool canBeIntractedWith = true;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public override void onFocus()
    { 
      
    }

    public override void onInteract()
    {
        if (canBeIntractedWith)
        {
            isOpen = !isOpen;

            Vector3 doorTransformDirecton = transform.TransformDirection(Vector3.forward);
            Vector3 playerTransformDirection = FirstPersonController.instance.transform.position - transform.position;

            float dot = Vector3.Dot(doorTransformDirecton, playerTransformDirection);

            animator.SetFloat("dot", dot);
            animator.SetBool("isOpen", isOpen);

            StartCoroutine(AutoCLose());
        }
    }

    public override void onLoseFocus()
    {
            
    }

    private IEnumerator AutoCLose()
    {
        while (isOpen)
        {    
            yield return new WaitForSeconds(3);

            if (Vector3.Distance(transform.position,FirstPersonController.instance.transform.position) > 3f)
            {
                isOpen = false;

                animator.SetFloat("dot", 0);
                animator.SetBool("isOpen", isOpen);
            }
        }
    }

    private void Animator_LockInteraction()
    {
        canBeIntractedWith = false;
    } 
    private void Animator_UnLockInteraction()
    {
        canBeIntractedWith = true;
    }

}
