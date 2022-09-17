using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCrawler : Enemy
{
    public float lookRadius = 10f;
    private Transform target;
    private NavMeshAgent Agent;
    private Animator animator;

    private void Start()
    {
        target = FirstPersonController.instance.transform;
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }



    private void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= lookRadius)
        {
            
            animator.SetBool("near",true);

            Agent.SetDestination(target.position);
        }
        else
            animator.SetBool("near", false);

        if (distance <= Agent.stoppingDistance)
        {
            FaceTarget();
        }

        animator.SetFloat("isStop", distance);
    }


    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        //normalized: yonunu koruyo ama uzunlugu 1 kaliyo ///mesela burada yon onemli o yuzden yaptik

        Quaternion LookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0,direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, Time.deltaTime * 5f);

    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }



}
