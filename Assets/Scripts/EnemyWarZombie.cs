using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWarZombie : Enemy
{

    public AudioClip[] ZombieSounds;


    private void Update()
    {
        if (!isDead)
        {
            if (useFootSteps)
                HandleFootSteps();
            if (useSetDistance)
                HandlesetDistance();

            SoundEffectCheck();
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, lookRadius);
    //}

    public void SoundEffectCheck()
    {
        if (Distance <= lookRadius)
        {
            if (MainAudioSource.clip != ZombieSounds[0])
            {
                MainAudioSource.clip = ZombieSounds[0];
                MainAudioSource.Play();
            }
        }
        else
        {
            if (MainAudioSource.clip != ZombieSounds[1])
            {
                MainAudioSource.clip = ZombieSounds[1];
                MainAudioSource.Play();
            }
        }
    }
}
