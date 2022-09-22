using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGhoul : Enemy
{

    private void Update()
    {
       



        if (useFootSteps)
            HandleFootSteps();

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= lookRadius)
        {
            Agent.isStopped = false;
            animator.SetBool("near", true);

            Agent.SetDestination(target.position);

            base.FaceTarget();
        }
        else
        {
            animator.SetBool("near", false);

            Agent.isStopped = true;
        }
            

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
