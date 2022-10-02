using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadZone : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
          //  SceneManager.LoadScene("ArtScene");
            SceneManager.LoadScene("TestScene");
        }
    }
}
