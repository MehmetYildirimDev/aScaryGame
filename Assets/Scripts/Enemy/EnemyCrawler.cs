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


    public bool useFootSteps = true;
    [Header("Foot Steps Parameters")]
    public float baseStepSpeed = 0.5f;
    public float CrawlerStepSpeed = 0.25f;
    public AudioSource audioSource = default;
    public AudioClip[] woodClips = default;
    public AudioClip[] NormalClips = default;
    public AudioClip[] MetalClips = default;
    public AudioClip[] grassClips = default;
    public float footStepTimer = 0;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        target = FirstPersonController.instance.transform;
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }



    private void Update()
    {

        FaceTarget();//random yurumek icin faydasiz

        if (useFootSteps)
            HandleFootSteps();
        ///random yurutmek icin
        //        Agent.SetDestination(new Vector3(Random.Range(transform.position.x, transform.position.x + 4),0, Random.Range(transform.position.z, transform.position.z + 4)));


        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= lookRadius)
        {

            animator.SetBool("near", true);

            Agent.SetDestination(target.position);
        }
        else
            animator.SetBool("near", false);

        //if (distance <= Agent.stoppingDistance)
        //{
        //    FaceTarget();
        //}

        animator.SetFloat("isStop", distance);
    }


    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        //normalized: yonunu koruyo ama uzunlugu 1 kaliyo ///mesela burada yon onemli o yuzden yaptik

        Quaternion LookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, Time.deltaTime * 5f);

    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.DrawRay(transform.position + new Vector3(0, 1, 0), Vector3.down);
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

            footStepTimer = CrawlerStepSpeed;
        }


    }
}
