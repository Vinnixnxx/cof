using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.VFX;
using EZCameraShake;

public class GunSystem : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float timeBetweenShooting;
    [SerializeField] private float spreadX;
    [SerializeField] private float spreadY;
    [SerializeField] private float range;
    [SerializeField] private float reloadTime;
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private int magazineSize;
    [SerializeField] private int bulletsPerShot;
    [SerializeField] private bool allowButtonHold;

    private int bulletsLeft;
    private int bulletsShot;


    bool isShooting;
    bool isReadyToShoot;
    bool isReloading;

    [SerializeField] private Camera fpsCam;
    [SerializeField] private RaycastHit hit;
    [SerializeField] private LayerMask Enemy;

    
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject muzzlePos;
    [SerializeField] private ParticleSystem bulletImpact;
    [SerializeField] private CameraShaker cameraShaker;
    [SerializeField] private float camShakeMagnitude;
    [SerializeField] private float camShakeRoughness;
    [SerializeField] private float camShakeDuration;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string nameOfGun;

    


    private void Awake()
    {
        bulletsLeft = magazineSize;
        isReadyToShoot = true;
        bulletImpact.Stop();
    }

    private void Update()
    {
        MyInput();
        text.SetText(bulletsLeft + " / " + magazineSize);

        muzzleFlash.transform.position = muzzlePos.transform.position;
        muzzleFlash.transform.rotation = muzzlePos.transform.rotation;
    }

    private void MyInput()
    {
        if (allowButtonHold) 
        {
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading) {
            Reload();
        }

        if (isReadyToShoot && isShooting && !isReloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerShot;
            Shoot();

            
        }

        if (bulletsLeft == 0 && !isReloading && isShooting)
        {
            FindObjectOfType<AudioManager>().Play(nameOfGun + "_dead_trigger");
        }

    }

    private void Shoot()
    {
        float x = 0;
        float y = 0;


        if (Input.GetKey(KeyCode.Mouse1))
        {
            x = 0;
            y = 0;
            FindObjectOfType<Animator>().Play("RecoilScope", 0, 0.0f);
        }
        if (!Input.GetKey(KeyCode.Mouse1))
        {
            x = Random.Range(-spreadX, spreadX);
            y = Random.Range(-spreadY, spreadY);
            FindObjectOfType<Animator>().Play("RecoilNoScope", 0, 0.0f);
        }

        isReadyToShoot = false;

        

        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);
        


        if (Physics.Raycast(fpsCam.transform.position, direction, out hit, range, Enemy))
        {
            Debug.Log(hit.collider.name);

            if (hit.collider.CompareTag("Enemy"))
            {
                
                hit.collider.GetComponent<EnemyAI>().TakeDamage(damage);
            }
        }

        if(Physics.Raycast(fpsCam.transform.position, direction, out hit, range))
        {
            Debug.Log(hit.collider.name);
            bulletImpact.transform.position = hit.point;          
            bulletImpact.transform.rotation = fpsCam.transform.rotation;
            

        }
        ParticleSystem newImpact = Instantiate(bulletImpact);
        Destroy(newImpact.transform.gameObject, 1.0f);


        cameraShaker.ShakeOnce(camShakeMagnitude, camShakeRoughness, 0f, camShakeDuration);
        muzzleFlash.Play();
        
        

        //sound

        FindObjectOfType<AudioManager>().Play(nameOfGun + "_shoot");
         
        bulletsShot--;
        bulletsLeft--;
        Invoke("ResetShot",timeBetweenShooting);
        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
        
    }


    private void ResetShot()
    {
        isReadyToShoot = true;
    }
    private void Reload()
    {
        isReloading = true;
        FindObjectOfType<AudioManager>().Play(nameOfGun+"_reload");
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        isReloading = false;
    }
    


}
