using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGhoul : Enemy
{




    private void Update()
    {
       base.FaceTarget();



        if (useFootSteps)
            HandleFootSteps();

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= lookRadius)
        {

            animator.SetBool("near", true);

            Agent.SetDestination(target.position);
        }
        else
            animator.SetBool("near", false);

        //if (distance <= Agent.stoppingDistance)
        //{
        //    FaceTarget();
        //}

        animator.SetFloat("isStop", distance);

    }

    


    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, lookRadius);
    //}
 


}
