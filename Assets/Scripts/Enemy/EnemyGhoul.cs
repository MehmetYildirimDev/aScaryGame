using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGhoul : Enemy
{

    public float lookRadius = 10f;
    private Transform target;
    private NavMeshAgent Agent;

    public bool useFootSteps = true;
    [Header("Foot Steps Parameters")]
    public float baseStepSpeed = 0.5f;
    public AudioSource audioSource = default;
    public AudioClip[] woodClips = default;
    public AudioClip[] NormalClips = default;
    public AudioClip[] MetalClips = default;
    public AudioClip[] grassClips = default;
    public float footStepTimer = 0;

    private void Start()
    {
        target = FirstPersonController.instance.transform;
        Agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }



    private void Update()
    {
        
        if (useFootSteps)
            HandleFootSteps();

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= lookRadius)
        {
            Agent.SetDestination(target.position);
        }

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


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
    public void HandleFootSteps()
    {

        if (Agent.velocity.magnitude <= 0) return;
        //if (currentInput == Vector2.zero) return;

        footStepTimer -= Time.deltaTime;

        if (footStepTimer <= 0)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3f))
            {
                switch (hit.collider.tag)
                {
                    case "FootSteps/Grass":
                        audioSource.PlayOneShot(grassClips[Random.Range(0, grassClips.Length - 1)]);
                        break;
                    case "FootSteps/Metal":
                        audioSource.PlayOneShot(MetalClips[Random.Range(0, MetalClips.Length - 1)]);
                        break;
                    case "FootSteps/Wood":
                        audioSource.PlayOneShot(woodClips[Random.Range(0, woodClips.Length - 1)]);
                        break;

                    default:
                        audioSource.PlayOneShot(NormalClips[Random.Range(0, NormalClips.Length - 1)]);
                        break;
                }
            }

            footStepTimer = baseStepSpeed;
        }


    }

}
