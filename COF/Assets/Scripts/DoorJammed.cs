using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorJammed : Interactable
{
    public override void OnFocus()
    {
        print("LOOKIN AT THE DOOR JAMMED!");
    }

    public override void OnInteract()
    {
        print("WOW IT REALLY IS JAMMED!");
    }

    public override void OnLoseFocus()
    {
        print("U NOT LOOKIN'!");
    }
}
