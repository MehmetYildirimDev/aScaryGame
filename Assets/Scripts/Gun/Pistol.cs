using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{


    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            animator.Play("shot");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.Play("reload");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.Play("punc");
        }
    }
}
