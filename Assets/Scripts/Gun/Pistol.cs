using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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


    [Header("Reload System")]
    public int Totalbullet = 90;
    public int clipbullet = 30;
    public int M4BULLET = 30;
    public int empty = 0;
    public float ReloadTime = 1.17f;
    private bool isReloding = false;
    private bool canShoot = true;
    public Text clip;
    public Text TotalAmmo;

    private void Start()
    {
        animator = GetComponent<Animator>();
        laserLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        fpsCam = GetComponentInParent<Camera>();

    }

    private void Update()
    {
        clip.text = clipbullet.ToString();

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
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > nextFire && canShoot)
        {
            clipbullet -= 1;
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
        #endregion




        if (Input.GetKeyDown(KeyCode.R) || clipbullet <= 0 && Totalbullet > 0)
        {
            StartCoroutine(ReloadClip());
            return;


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

        TotalAmmo.text = Totalbullet.ToString();

        isReloding = false;
    }
}
