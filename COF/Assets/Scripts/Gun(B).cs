using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private KeyCode fire = KeyCode.Mouse0;
    [SerializeField] private Camera camera;
    [SerializeField] private ParticleSystem muzzleFlash;


    // Update is called once per frame
    void Update()
    {
        if (true)
        {
            if (Input.GetKeyDown(fire))
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        muzzleFlash.Play();
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        
    }

    private void GunCrosshair()
    {

    }

}
