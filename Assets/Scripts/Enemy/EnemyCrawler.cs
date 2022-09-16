using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCrawler : Enemy
{
    public float lookRadius = 10f;
    private Transform target;
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        target = FirstPersonController.instance.transform;
        navMeshAgent = GetComponent<NavMeshAgent>(); 
    }



    private void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= lookRadius)
        {
            navMeshAgent.SetDestination(target.position);
        }




    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }



}
