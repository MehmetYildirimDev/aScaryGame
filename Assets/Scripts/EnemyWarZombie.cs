using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWarZombie : Enemy
{
    private void Update()
    {
        if (!isDead)
        {
            if (useFootSteps)
                HandleFootSteps();
            if (useSetDistance)
                HandlesetDistance();
        }


    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, lookRadius);
    //}

   
}
