using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDoorController : MonoBehaviour
{
    private Animator doorAnim;
    public bool doorOpen = false;
    public bool flip = false;

    
    
    private void Awake()
    {

    }

    public void Update()
    {
        if (doorOpen)
        {
            this.GetComponent<MeshCollider>().enabled = false;
        }
        else
        {
            this.GetComponent<MeshCollider>().enabled = true;
        }
    }



}
