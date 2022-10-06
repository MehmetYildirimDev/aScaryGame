using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public int SelectedWeapon = 0;
    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();

    }



    // Update is called once per frame
    void Update()
    {
        int previousSelectedweapon = SelectedWeapon;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (SelectedWeapon >= transform.childCount - 1)
                SelectedWeapon = 0;
            else
                SelectedWeapon++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (SelectedWeapon <= 0)
                SelectedWeapon = transform.childCount -1 ;
            else
                SelectedWeapon--;
        }

        if (previousSelectedweapon != SelectedWeapon)
        {
            SelectWeapon();
        }
    }
    private void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in this.transform)
        {
            if (i == SelectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
