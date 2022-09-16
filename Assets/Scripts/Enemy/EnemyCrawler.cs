using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCrawler : Enemy
{
    [SerializeField] private float StartWaitTime = 4f;
    [SerializeField] private float timeToRotate = 4f;
    [SerializeField] private float speedWalk = 6f;
    [SerializeField] private float speedRun = 9f;

    [SerializeField] private float viewRadius = 15f;
    [SerializeField] private float viewAngle = 90f;

    public NavMeshAgent navMeshAgent;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void LookingPlayer()
    {
        navMeshAgent.SetDestination(FirstPersonController.instance.transform.position);
    }






}
