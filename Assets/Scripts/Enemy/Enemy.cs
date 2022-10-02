using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    public bool isDead = false;
    public int currentHealt = 3;
    public int Healt;


    protected Transform target;

    public float lookRadius = 10f;

    public bool useFootSteps = true;
    [Header("Foot Steps Parameters")]
    public float baseStepSpeed = 0.5f;
    public float CrawlerStepSpeed = 0.25f;
    public AudioSource FootStepaudioSource = default;
    public AudioClip[] woodClips = default;
    public AudioClip[] NormalClips = default;
    public AudioClip[] MetalClips = default;
    public AudioClip[] grassClips = default;
    public float footStepTimer = 0;

    public bool useSetDistance = true;
    protected float Distance;
    protected NavMeshAgent Agent;
    protected Animator animator;


    [Header("Field of View")]
    public float FovRadius;
    [Range(0, 360)] public float FovAngle;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public bool canSeePlayer;

    [Header("Sounds System")]
    public AudioSource MainAudioSource;
    public AudioClip takeDamageSfx;
    private void Start()
    {
        Healt = currentHealt;
        isDead = false;
        Agent = GetComponent<NavMeshAgent>();
        MainAudioSource = GetComponent<AudioSource>();
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

    public void HandlesetDistance()
    {
        Distance = Vector3.Distance(transform.position, target.position);

        if (Distance <= lookRadius || canSeePlayer || Healt != currentHealt) //hasar aldiginda da gelmesi gerek
        {
            Agent.isStopped = false;
            animator.SetBool("near", true);

            Agent.SetDestination(target.position);

            FaceTarget();
        }
        else
        {
            animator.SetBool("near", false);

            Agent.isStopped = true;
        }


        //if (distance <= Agent.stoppingDistance)
        //{
        //    FaceTarget();
        //}

        animator.SetFloat("isStop", Distance);

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
                        FootStepaudioSource.PlayOneShot(grassClips[Random.Range(0, grassClips.Length - 1)]);
                        break;
                    case "FootSteps/Metal":
                        FootStepaudioSource.PlayOneShot(MetalClips[Random.Range(0, MetalClips.Length - 1)]);
                        break;
                    case "FootSteps/Wood":
                        FootStepaudioSource.PlayOneShot(woodClips[Random.Range(0, woodClips.Length - 1)]);
                        break;

                    default:
                        FootStepaudioSource.PlayOneShot(NormalClips[Random.Range(0, NormalClips.Length - 1)]);
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

    public void onDamage(int damageAmount)
    {
        AudioSource.PlayClipAtPoint(takeDamageSfx, transform.position, 1f);
        Healt -= damageAmount;
        if (Healt <= 0)//isdead
        {

            isDead = true;
            animator.enabled = false;
            KinematicState();
            Agent.speed = 0f;
            //MainAudioSource.Stop();
            //Destroy(this.gameObject, 3f);

        }
    }

    //animasyonda vuruyorsa burayi cagiriyo ve eger hasar al dogruysa(o da kolu degdiyse true oluyo) hasar verito ve surekli hasar alma olayini cozyuoruz
    public void onDamagePlayer()
    {
        if (FirstPersonController.instance.ZombieTakeDamage)
        {
            FirstPersonController.instance.ApplyDamage(10);
        }
        FirstPersonController.instance.ZombieTakeDamage = false;
    }




    //fizikten etkilenip etkilenmeme 
    public void KinematicState()
    {
        Rigidbody[] rigidbodies = this.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = false;
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
