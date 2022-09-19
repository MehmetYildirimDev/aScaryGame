using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCrawler : Enemy
{

    private void Update()
    {

        if (useFootSteps)
            HandleFootSteps();
        
        ///random yurutmek icin
        //        Agent.SetDestination(new Vector3(Random.Range(transform.position.x, transform.position.x + 4),0, Random.Range(transform.position.z, transform.position.z + 4)));


        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= lookRadius)
        {
            animator.SetBool("near", true);

            Agent.SetDestination(target.position);

            base.FaceTarget();
        }
        else
            animator.SetBool("near", false);

        //if (distance <= Agent.stoppingDistance)
        //{
        //    base.FaceTarget();
        //}

        animator.SetFloat("isStop", distance);
    }

}
