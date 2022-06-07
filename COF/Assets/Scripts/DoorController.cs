using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorController : Interactable
{

    public Image crosshairDefault;
    public Image crosshairUse;
    public Image crosshairLocked;
    [SerializeField] private bool isLocked;
    [SerializeField] private bool isBroken;
    [SerializeField] private bool doorOpen;
    [SerializeField] private bool flip;
    [SerializeField] private float timeToMove = 0.25f;
    [SerializeField] private bool Animating = false;
    private string[] openSounds = { "doorOpen1", "doorOpen2", "doorOpen3" };
    private string[] closeSounds = { "doorClose1", "doorClose2", "doorClose3" };
    public override void OnFocus()
    {
        if (isLocked)
        {
            crosshairDefault.transform.gameObject.GetComponent<Image>().enabled = false;
            crosshairUse.transform.gameObject.GetComponent<Image>().enabled = false;
            crosshairLocked.transform.gameObject.GetComponent<Image>().enabled = true;
        }
        else
        {
            crosshairDefault.transform.gameObject.GetComponent<Image>().enabled = false;
            crosshairUse.transform.gameObject.GetComponent<Image>().enabled = true;
            crosshairLocked.transform.gameObject.GetComponent<Image>().enabled = false;
        }     
    }

    public override void OnInteract()
    {

        if (!Animating && !isLocked && !isBroken)
        {
            HandleAnimation();
            
        }
        else if (!Animating && isLocked && !isBroken)
        {
            HandleLockedDoor();
        }
        else if (!Animating && isBroken && !isLocked)
        {
            HandleBrokenDoor();
        }
        
    }

    public override void OnLoseFocus()
    {
        crosshairDefault.transform.gameObject.GetComponent<Image>().enabled = true;
        crosshairUse.transform.gameObject.GetComponent<Image>().enabled = false;
    }

    //HandleUnlock(): Ha van kulcsod akkor => isLocked = false;
    

    private void HandleLockedDoor()
    {
        Debug.Log("IT'S LOCKED--------------------"); //text on hud: It's locked.
    }

    private void HandleBrokenDoor()
    {
        Debug.Log("IT'S BROKEN--------------------");   //text on hud: It won't open.; It won't move.; It's broken.; stb...
    }

    private void HandleAnimation()
    {
    UnityEngine.Random.Range(0, 2);

        if (!doorOpen && !flip)
        {
            StartCoroutine(OpenDoorAnim());
            doorOpen = true;
            FindObjectOfType<AudioManager>().Play(openSounds[(int)UnityEngine.Random.Range(0, 2)]);


        }
        else if (doorOpen && !flip)
        {
            StartCoroutine(CloseDoorAnim());
            doorOpen = false;
            FindObjectOfType<AudioManager>().Play(closeSounds[(int)UnityEngine.Random.Range(0, 2)]);
        }
        //------------------------------------

        if (!doorOpen && flip)
        {
            StartCoroutine(CloseDoorAnim());
            doorOpen = true;
            
            FindObjectOfType<AudioManager>().Play(openSounds[(int)UnityEngine.Random.Range(0, 2)]);
        }
        else if (doorOpen && flip)
        {
            StartCoroutine(OpenDoorAnim());
            doorOpen = false;
            FindObjectOfType<AudioManager>().Play(closeSounds[(int)UnityEngine.Random.Range(0, 2)]);
        }
    }

    private IEnumerator OpenDoorAnim()
    {
        Animating = true;
        Quaternion fromRot = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        Quaternion toRot = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 90, transform.rotation.eulerAngles.z);
        float timeElapsed = 0;
        while (timeElapsed < timeToMove)
        {
            transform.rotation = Quaternion.Slerp(fromRot, toRot, timeElapsed / timeToMove);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = toRot;
        Animating = false;
    }
    private IEnumerator CloseDoorAnim()
    {
        Animating = true;
        Quaternion fromRot = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        Quaternion toRot = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 90, transform.rotation.eulerAngles.z);
        float timeElapsed = 0;
        while (timeElapsed < timeToMove)
        {
            transform.rotation = Quaternion.Slerp(fromRot, toRot, timeElapsed / timeToMove);
            timeElapsed += Time.deltaTime;
            Debug.Log(transform.rotation);
            yield return null;
        }

        transform.rotation = toRot;
        Animating = false;
    }
}
