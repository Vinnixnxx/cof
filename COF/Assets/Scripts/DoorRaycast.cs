using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DoorRaycast : MonoBehaviour
{
    [SerializeField] private int rayLength = 5;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private string excludeLayerName = null;
    private MyDoorController raycastedObj;
    [SerializeField] private float timeToMove = 0.25f;

    [SerializeField] private KeyCode openDoorKey = KeyCode.Mouse0;
    [SerializeField] private Image crosshairDefault = null;
    [SerializeField] private Image crosshairUse = null;
    private bool isCrosshairActive;
    private bool doOnce;
    private bool Animating = false;
    

    private const string interactableTag = "Interact";

    private void Update()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        


        int mask = 1 << LayerMask.NameToLayer(excludeLayerName) | layerMaskInteract.value;

        if (Physics.Raycast(transform.position, fwd, out hit, rayLength, mask))
        {
            if (hit.collider.CompareTag(interactableTag))
            {
                if (!doOnce)
                {
                    raycastedObj = hit.collider.gameObject.GetComponent<MyDoorController>();
                    CrosshairChange(true);
                }
                isCrosshairActive = true;
                doOnce = true;

                if (Input.GetKeyDown(openDoorKey) && !Animating)
                {
                    HandleAnimation();
                }
            }
        }
        else
        {
            if (isCrosshairActive)
            {
                CrosshairChange(false);
                doOnce = false;
            }
        }
    }

    private void CrosshairChange(bool on)
    {
        if (on && !doOnce)
        {
            crosshairDefault.transform.gameObject.GetComponent<Image>().enabled = false;
            crosshairUse.transform.gameObject.GetComponent<Image>().enabled = true;
        }
        else
        {
            crosshairDefault.transform.gameObject.GetComponent<Image>().enabled = true;
            crosshairUse.transform.gameObject.GetComponent<Image>().enabled = false;
            isCrosshairActive = false;
        }
    }

    private void HandleAnimation()
    {
        if (!raycastedObj.doorOpen && !raycastedObj.flip)
        {
            StartCoroutine(OpenDoorAnim());
            raycastedObj.doorOpen = true;
        }
        else if (raycastedObj.doorOpen && !raycastedObj.flip)
        {
            StartCoroutine(CloseDoorAnim());
            raycastedObj.doorOpen = false;
        }
        //------------------------------------

        if (!raycastedObj.doorOpen && raycastedObj.flip)
        {
            StartCoroutine(CloseDoorAnim());
            raycastedObj.doorOpen = true;
        }
        else if (raycastedObj.doorOpen && raycastedObj.flip)
        {
            StartCoroutine(OpenDoorAnim());
            raycastedObj.doorOpen = false;
        }
    }

    private IEnumerator OpenDoorAnim()
    {
        Animating = true;
        Quaternion fromRot = Quaternion.Euler(raycastedObj.transform.rotation.eulerAngles.x, raycastedObj.transform.rotation.eulerAngles.y, raycastedObj.transform.rotation.eulerAngles.z);
        Quaternion toRot = Quaternion.Euler(raycastedObj.transform.rotation.eulerAngles.x, raycastedObj.transform.rotation.eulerAngles.y + 90, raycastedObj.transform.rotation.eulerAngles.z);
        float timeElapsed = 0;
        while (timeElapsed < timeToMove)
        {
            raycastedObj.transform.rotation = Quaternion.Slerp(fromRot, toRot, timeElapsed / timeToMove);
            timeElapsed += Time.deltaTime;
            Debug.Log(toRot);
            yield return null;
        }
        raycastedObj.transform.rotation = toRot;
        Animating = false;
    }
    private IEnumerator CloseDoorAnim()
    {
        Animating = true;
        Quaternion fromRot = Quaternion.Euler(raycastedObj.transform.rotation.eulerAngles.x, raycastedObj.transform.rotation.eulerAngles.y, raycastedObj.transform.rotation.eulerAngles.z);
        Quaternion toRot = Quaternion.Euler(raycastedObj.transform.rotation.eulerAngles.x, raycastedObj.transform.rotation.eulerAngles.y -90, raycastedObj.transform.rotation.eulerAngles.z);
        float timeElapsed = 0;
        while (timeElapsed < timeToMove)
        {
            raycastedObj.transform.rotation = Quaternion.Slerp(fromRot, toRot, timeElapsed / timeToMove);
            timeElapsed += Time.deltaTime;
            Debug.Log(raycastedObj.transform.rotation);
            yield return null;
        }

        raycastedObj.transform.rotation = toRot;
        Animating = false;
    }


}
