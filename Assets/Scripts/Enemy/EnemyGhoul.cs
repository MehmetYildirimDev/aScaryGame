using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGhoul : Enemy
{

    public float lookRadius = 10f;
    private Transform target;
    private NavMeshAgent Agent;
    private Animator animator;

    public bool useFootSteps = true;
    [Header("Foot Steps Parameters")]
    public float baseStepSpeed = 0.5f;
    public AudioSource audioSource = default;
    public AudioClip[] woodClips = default;
    public AudioClip[] NormalClips = default;
    public AudioClip[] MetalClips = default;
    public AudioClip[] grassClips = default;
    public float footStepTimer = 0;


    [Header("Field of View")]
    public float FovRadius;
    [Range(0, 360)] public float FovAngle;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public bool canSeePlayer;


    private void Start()
    {
        target = FirstPersonController.instance.transform;
        Agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        StartCoroutine(FovRuotine());
    }



    private void Update()
    {
        FaceTarget();

        if (useFootSteps)
            HandleFootSteps();

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


    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, lookRadius);
    //}
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

    private IEnumerator FovRuotine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FOVChechk();

        }

    }

    private void FOVChechk()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, FovRadius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directonToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directonToTarget) < FovAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directonToTarget, distanceToTarget, obstructionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)//daha once gorduyse býrakýyo
            canSeePlayer = false;
    }
}
