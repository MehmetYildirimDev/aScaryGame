using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    

    public float Healt=100f;
     public float Damage=10f;
     public float WalkSpeed=5f;

    protected Transform target;

    public float lookRadius = 10f;

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

    protected NavMeshAgent Agent;
    protected Animator animator;


    [Header("Field of View")]
    public float FovRadius;
    [Range(0, 360)] public float FovAngle;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public bool canSeePlayer;

    private void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        StartCoroutine(FovRuotine());
        target = FirstPersonController.instance.transform;
    }

    protected void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        //normalized: yonunu koruyo ama uzunlugu 1 kaliyo ///mesela burada yon onemli o yuzden yaptik

        Quaternion LookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, Time.deltaTime * 5f);

    }


    protected virtual void HandleFootSteps()
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

    //protected void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, lookRadius);
    //    Gizmos.DrawRay(transform.position + new Vector3(0, 1, 0), Vector3.down);
    //}

    public int currentHealt = 3;
    public void onDamage(int damageAmount)
    {
        currentHealt -= damageAmount;
        if (currentHealt <= 0)
        {
            //animator.Play("Death");
            //Destroy(this.gameObject, 2f);
            gameObject.SetActive(false);
        }
    }
    protected IEnumerator FovRuotine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FOVChechk();

        }

    }

    protected void FOVChechk()
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
