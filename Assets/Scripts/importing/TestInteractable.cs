using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractable : Interactable
{
    public override void onFocus()//bakarken olanlar
    {
        print("Looking at" + gameObject.name);
    }

    public override void onInteract()//tusa bastigimizda
    {
        print(" interacted with " + gameObject.name);
    }

    public override void onLoseFocus()//bakmayi biraktik
    {
        print("stopped Looking at" + gameObject.name);
    }
}
