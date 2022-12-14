using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Pistol : MonoBehaviour
{
    [Header("Gun System")]
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


    [Header("Reload System")]
    public int Totalbullet = 90;
    public int clipbullet = 30;
    public int M4BULLET = 30;
    public int empty = 0;
    public float ReloadTime = 1.17f;
    [SerializeField] private bool isReloding = false;
    [SerializeField] private bool canShoot = true;
    [SerializeField] private bool DrawcanShoot = default;
    public static Action<float> onClip;
    public static Action<float> onTotalAmmo;
    // public Text clip;
    // public Text TotalAmmo;

    [Header("Particle System")]
    public ParticleSystem MuzzleFlash;

    [Header("impact System")]
    public GameObject[] bloods;
    public GameObject[] others;
    private void Start()
    {
        animator = GetComponent<Animator>();
        laserLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        fpsCam = GetComponentInParent<Camera>();

    }

    private void Update()
    {
        if (!FirstPersonController.instance.PlayerisDead)
        {
            onClip?.Invoke(clipbullet);

            if (isReloding) return;

            if (Totalbullet < 0 || clipbullet <= 0)
            {
                canShoot = false;
            }
            else
            {
                canShoot = true;
            }



            #region Shoot
            if (Input.GetKeyDown(KeyCode.Mouse0) && DrawcanShoot && Time.time > nextFire && canShoot)
            {
                Shoot();
            }
            #endregion




            if (Input.GetKeyDown(KeyCode.R) || clipbullet <= 0 && Totalbullet > 0)
            {
                if (clipbullet < M4BULLET)
                {
                    StartCoroutine(ReloadClip());
                    return;
                }

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
        

    }



    private void Shoot()
    {
        clipbullet -= 1;
        animator.Play("shot");
        MuzzleFlash.Play();

        nextFire = Time.time + fireRate;

        StartCoroutine(ShotEffect());

        Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));//ekranin ortasi

        RaycastHit hit;

        //laserLine.SetPosition(0, gunEnd.position);

        if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, WeaponRange))//out kulanmak donus bilgilerinden daha fazla bilgi donderirir
        {
           // laserLine.SetPosition(1, hit.point);


            /*
                  Enemy healt = hit.collider.GetComponent<Enemy>();
                           if (healt != null)
                           {
                                healt.transform.root.GetComponent<Enemy>().onDamage(GunDamage);
                              healt.onDamage(GunDamage);
                           }
             */


            if (hit.collider.CompareTag("enemy1"))
            {
                hit.collider.transform.root.GetComponent<Enemy>().onDamage(GunDamage);
                Debug.Log(hit.collider.name);

                GameObject impactGO = Instantiate(bloods[UnityEngine.Random.Range(0, bloods.Length - 1)], hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 1f);


            }
            else
            {
                GameObject impactGO = Instantiate(others[UnityEngine.Random.Range(0, others.Length - 1)], hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 1f);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * hitForce);
            }

            if (hit.collider.CompareTag("Lamp"))
            {
                hit.collider.gameObject.transform.GetChild(0).gameObject.SetActive(false);

            }
        }
        else
        {
            //laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * WeaponRange));
        }



    }

    private IEnumerator ShotEffect()
    {
        gunAudio.PlayOneShot(audioClips[0]);
       // laserLine.enabled = true;
        yield return shotDuration;

        //laserLine.enabled = false;
    }

    private void DrawAudioandStart()
    {
        gunAudio.PlayOneShot(audioClips[1]);
        DrawcanShoot = false;
    }  
    private void DrawFinish()
    {
        DrawcanShoot = true;
    }


    IEnumerator ReloadClip()
    {
        isReloding = true;

        animator.Play("reload");
        gunAudio.PlayOneShot(audioClips[2]);

        yield return new WaitForSeconds(ReloadTime);

        if (Totalbullet > 0)
        {
            empty = M4BULLET - clipbullet;

            if (empty > Totalbullet)
            {
                empty = Totalbullet;
            }
            Totalbullet -= empty;
            clipbullet += empty;
        }

        //   TotalAmmo.text = Totalbullet.ToString();
        onTotalAmmo?.Invoke(Totalbullet);


        isReloding = false;
    }
}
