using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGhoul : Enemy
{

    public float lookRadius = 10f;
    private Transform target;
    private NavMeshAgent Agent;

    private void Start()
    {
        target = FirstPersonController.instance.transform;
        Agent = GetComponent<NavMeshAgent>();
    }



    private void Update()
    {
        float distance = DistanceCalculate();


        if (distance <= Agent.stoppingDistance)
        {
            FaceTarget();
        }

    }

    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        //normalized: yonunu koruyo ama uzunlugu 1 kaliyo ///mesela burada yon onemli o yuzden yaptik

        Quaternion LookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation,LookRotation,Time.deltaTime*5f);

    }

    private float DistanceCalculate()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= lookRadius)
        {
            Agent.SetDestination(target.position);
        }
        return distance;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
