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
        else
        {
            if (MainAudioSource.clip != ZombieSounds[3])
            {
                MainAudioSource.clip = ZombieSounds[3];
                MainAudioSource.Play();
            }
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, lookRadius);
    //}

    public void SoundEffectCheck()
    {

        if (Distance <= Agent.stoppingDistance)
        {
            if (MainAudioSource.clip != ZombieSounds[2])
            {
                MainAudioSource.clip = ZombieSounds[2];
                MainAudioSource.Play();
            }
            return;
        }

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
