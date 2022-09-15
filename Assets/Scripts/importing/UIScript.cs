using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class UIScript : MonoBehaviour
{

    [SerializeField] private Text HealtText = default;
    [SerializeField] private Text StaminaText = default;

    private void OnEnable()
    {
        FirstPersonController.onDamage += UpdateHealt;
        FirstPersonController.onHeal += UpdateHealt;
        FirstPersonController.onStaminaChange += UpdateStamina;
    }
    private void OnDisable()
    {
        FirstPersonController.onDamage -= UpdateHealt;
        FirstPersonController.onHeal -= UpdateHealt;
        FirstPersonController.onStaminaChange -= UpdateStamina;
    }

    private void Start()
    {
        UpdateHealt(100);
        UpdateStamina(100);
    }

    private void UpdateHealt(float currentHealt)
    {
        HealtText.text = currentHealt.ToString("00");
    }

    private void UpdateStamina(float currentStamina)
    {
        StaminaText.text = currentStamina.ToString("00");
    }
}
