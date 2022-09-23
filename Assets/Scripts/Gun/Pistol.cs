using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    public int GunDamage = 1;
    public float fireRate = 0.25f;//tekrar ates edebilmek icin gecen sure
    public float WeaponRange = 50f;  //menzil
    public float hitForce = 100f;  //rigidbodyse olan cise kuvvet uygulucaz
    public Transform gunEnd;

    private Camera fpsCam;
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);//lazer gorunumunun suresi

    private LineRenderer laserLine;//duz cizgi ceker iki nokta arasina
    private float nextFire;//sonraki ates icn sure ?? 

    [Header("Audio")]
    private AudioSource gunAudio;
    public AudioClip[] audioClips;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        laserLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        fpsCam = GetComponentInParent<Camera>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time>nextFire)
        {
            animator.Play("shot");

            nextFire = Time.time + fireRate;

            StartCoroutine(ShotEffect());

            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));//ekranin ortasi

            RaycastHit hit;

            laserLine.SetPosition(0, gunEnd.position);
            
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, WeaponRange))//out kulanmak donus bilgilerinden daha fazla bilgi donderirir
            {
                laserLine.SetPosition(1, hit.point);


                Enemy healt = hit.collider.GetComponent<Enemy>();
                if (healt != null)
                {
                    healt.onDamage(GunDamage);
                }

                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * hitForce);
                }
            }
            else
            {
                laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * WeaponRange)); 
            }
            


        }



        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.Play("reload");
            gunAudio.PlayOneShot(audioClips[2]);
        }


        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.Play("punc");
            gunAudio.PlayOneShot(audioClips[3]);
            //todo:dusmana carparsa 4. index
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.Play("draw");
        }

    }

    private IEnumerator ShotEffect()
    {
        gunAudio.PlayOneShot(audioClips[0]);
        laserLine.enabled = true;
        yield return shotDuration;

        laserLine.enabled = false; 
    }
        
    private void DrawAudio()
    {
        gunAudio.PlayOneShot(audioClips[1]);
    }
}
