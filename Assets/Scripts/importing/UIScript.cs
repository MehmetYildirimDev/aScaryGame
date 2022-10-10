using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class UIScript : MonoBehaviour
{

    [SerializeField] private Text HealtText = default;
    [SerializeField] private Text StaminaText = default; 
    [SerializeField] private Text clipText = default; 
    [SerializeField] private Text TotalAmmoText = default;
    [SerializeField] private Text ChargeText = default;




    private void OnEnable()
    {
        FirstPersonController.onDamage += UpdateHealt;
        FirstPersonController.onHeal += UpdateHealt;
        FirstPersonController.onStaminaChange += UpdateStamina;
        Pistol.onClip += UpdateClip;
        Pistol.onTotalAmmo += UpdateTotalAmmo;
        Flashlight_PRO.onChargeChange += UpdateCharge;
    }



    private void OnDisable()
    {
        FirstPersonController.onDamage -= UpdateHealt;
        FirstPersonController.onHeal -= UpdateHealt;
        FirstPersonController.onStaminaChange -= UpdateStamina; 
        Pistol.onClip -= UpdateClip;
        Flashlight_PRO.onChargeChange -= UpdateCharge;
    }

    private void Start()
    {
        UpdateHealt(100);
        UpdateStamina(100);
        UpdateTotalAmmo(90);//BURAYA DÝKKAT
        UpdateCharge(100);
    }

    private void UpdateHealt(float currentHealt)
    {
        HealtText.text = currentHealt.ToString("00");
        
    }

    private void UpdateStamina(float currentStamina)
    {
        StaminaText.text = currentStamina.ToString("00");
    }

    private void UpdateCharge(float currentCharge)
    {
        ChargeText.text = currentCharge.ToString("00");
    }
    private void UpdateClip(float clip)
    {
        clipText.text = clip.ToString("00") + "/";
    }

    private void UpdateTotalAmmo(float TotalAmmo)
    {
        TotalAmmoText.text = TotalAmmo.ToString("00");
    }

}
