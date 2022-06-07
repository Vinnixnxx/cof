using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{


    private bool IsSprinting => CanSprint && Input.GetKey(sprintKey); //ha a játékos földön van, szabad sprintelni és meg van nyomva a gomb, akkor legyen igaz. 
    private bool ShoudJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded; //ha megnyomjuk a gombot és földön vagyunk akkor legyen igaz
    private bool ShouldCrouch => characterController.isGrounded && Input.GetKeyDown(crouchKey) && !duringCrouchAnimation; //ha földön vagyunk, lenyomjuk a gombot és nem animáció közben vagyunk, akkor legyen igaz


    [Header("Functional Options")] //logikai változók. ha egy adott hamis, akkor abszolút nem lehet használni azt az adott mozgást.
    [SerializeField] private bool CanSprint = true;   
    [SerializeField] private bool CanMove = true;
    [SerializeField] private bool CanJump = true;
    [SerializeField] private bool CanCrouch = true;
    [SerializeField] private bool CanUseHeadBob = true;
    [SerializeField] private bool WillSlideOnSlopse = true;
    [SerializeField] private bool CanZoom = true;
    [SerializeField] private bool CanInteract = true;
    [SerializeField] private bool CanLight = true;
    [SerializeField] private bool CantDrawCursor
    {
        get
        {
            if (Input.GetKey(zoomKey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Header("Controls")] //a billentyű teszteszabása
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse2;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode flashKey = KeyCode.F;


    [Header("Health")] //életerő paraméterei
    [SerializeField] private float maxHealth = 100f;//max élet
    [SerializeField] private float regenTime = 3;//regenráció kezdete előtti szünet
    [SerializeField] private float heathValueIncrement = 1; //mennyivel regenerálodjon a hp
    [SerializeField] private float healthTimeIncrement = 0.1f; //mennyi időnként regenerálódjon az élet
    private float currentHealth; //jelenlegi élet (a játék közben)
    private Coroutine regenHealthRoutine; //a regenerálódás ciklusa
    public static Action<float> OnTakeDamage; //mi történjen ha sebződönk
    public static Action<float> OnDamage; //mi történjen ha sebezünk
    public static Action<float> OnHeal; //mi történjen ha regenerálódunk

    [Header("Movement")] //mozgás paraméterei
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.0f;
    [SerializeField] private float slopeSpeed = 8f; //ha csúszunk le egy lejtőről, akkor milyen gyorsan tegyük azt. NEM LEHET TÖBB AZ ÉRTÉKE ENNÉL SEMELYIK MOZGÁSNAK!!!
    [SerializeField] private float gravity = 30.0f;
    [SerializeField] private float jumpForce = 2.0f;
    [SerializeField] private float crouchHeight = 1.0f; //guggolásnál mennyire magasan legyen a karakter "feje".
    [SerializeField] private float standHeight = 1.8f;  //álló helyzetben hol legyen a karakter "feje".
    [SerializeField] private float timeToCrouch = 0.25f; //mennyi idő alatt guggoljon le a karakter.
    [SerializeField] private Vector3 crouchCenter = new Vector3(0, 0.5f, 0); //karakter közepe guggolásnál.
    [SerializeField] private Vector3 standCenter = new Vector3(0, 0, 0); //karakter közepe álló helyzetben.
    private bool IsCrouching;   
    private bool duringCrouchAnimation; //megy-e a guggolás folyamat változója.

    [Header("Look")]//kamerapásztázás paraméterei.
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f; 
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 10)] private float scopeLookSpeedX = 2.0f;//zoomolás közbeni sebesség az egérrel az x tengelyen.
    [SerializeField, Range(1, 10)] private float scopeLookSpeedY = 2.0f;//zoomolás közbeni sebesség az egérrel az y tengelyen.
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;//mennyire nézhetsz fel
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;//mennyire nézhetsz le (MIVEL NEGÁLOM, CSAK POZITíV LEHET!!!)
    [SerializeField] private Camera playerCamera; //sose találod ki!

    [Header("Headbob")]//fej billegése mozgás közben
    //a fejmozgás erősségét és sebességét definiálja...
    [SerializeField] private float walkBobSpeed = 5f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 8f;
    [SerializeField] private float sprintBobAmount = 0.08f;
    [SerializeField] private float crouchBobSpeed = 3f;
    [SerializeField] private float crouchBobAmount = 0.02f;
    [SerializeField] private float walkBobSpeedRot = 5f;
    [SerializeField] private float walkBobAmountRot = 0.05f;
    [SerializeField] private float sprintBobSpeedRot = 8f;
    [SerializeField] private float sprintBobAmountRot = 0.08f;
    [SerializeField] private float crouchBobSpeedRot = 3f;
    [SerializeField] private float crouchBobAmountRot = 0.02f;
    private float defaultYPos = 0;//a fej eredeti y pozíciója
    private float defaultZRot = 0;//a fej eredeti z rotációja

    private float timerPos = 0; //a fej pozíciójának időzítője.. mire ez egyenlő valamennyivel, addig mozog a fej.
    private float timerRot = 0; //same shit csak forgástengellyel

    public Image crosshairDefault = null; //az alap kurzor 
    public Image crosshairUse = null;   //a "use" kurzor

    // SLIDING
    private Vector3 hitPointNormal;//a meredek objektumokról való lecsúszásért felelős vector
    private bool IsSliding //azt vizsgálja, hogy éppen csúszunk-e. ha a karakter földön van,
                           //akkor lelő egy raycastet a földre és megvizsgálja hogy nagyobb-e az alattunk lévő föld szöge, mint a limit, ami be van állítva a character controllerbe 
    {
        get
        {
            if (characterController.isGrounded && Physics.Raycast(transform.position,Vector3.down, out RaycastHit slopeHit, 2f))
            {
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit;
            }
            else
            {
                return false;
            }
        }
    }

    [Header("Zoom")] //zoomolás paraméterjei
    [SerializeField] private float timeToZoom = 0.3f; //mennyi idő a zoomolás
    [SerializeField] private float zoomFOV = 30f; //mekkora legyen a kamera látószöge
    private float defaultFOV;   //alap látószög
    private Coroutine zoomRoutine; //zoomolás "ciklus"
    private Coroutine crouchRoutine; //guggolás "ciklus"
    private CharacterController characterController; //vajon... mi lehet ez? hmm

    private Vector3 moveDirection; //mozgás vektorja
    private Vector2 currentInput;  //jelenlegi (játékbeli) mozgás vektorja

    private float rotationX = 0.0f; //egér forgásáért felelős. am fingom sincs mi ez.


    [Header("Interaction")] //Interspárakció
    [SerializeField] private Vector3 interactionRayPoint = default; //az a pont amire nézni fog majd a raycast
    [SerializeField] private float interactionDistance = default;   //milyen messze nyúlik a karunk ;)
    [SerializeField] private LayerMask interactionLayer = default;  //milyen réteggel közösülhetünk
    private Interactable currentInteractable; //mi az amivel épp közösülünk

    [Header("Light source")] //jézus fényének beállítása
    [SerializeField] private GameObject light; //ez AZ! Jézus fénye! Amit a kezedben tarthatsz!
    private bool isFlashOn = true; //be van e kapcsolva erik

    [Header("Weapon")]//vepön settings
    [SerializeField] private bool HasWeapon = true; //van e nálunk fegyver. FONTOS!: még nincs beállíva, hogy mi van akkor amikor nincs nálunk fegyver. azzal később kellene foglalkozni.
    [SerializeField] private GameObject weapon; //maga a fegyver.. nyugodtan tegezz
    private Animator weaponAnim; //fegyver animációja

    //Akkor kapcsol az iq bajnok metódusunk, ha valaki megbasz minket
    private void OnEnable()
    {
        OnTakeDamage += ApplyDamage;
    }
    //ellentétesen cselekszik az eggyel fentebbihez képest
    private void OnDisable()
    {
        OnTakeDamage -= ApplyDamage;
    }


    // Start is called before the first frame update <- beszélsz angolul?
    void Awake() //beállítok mindennek - aminek fontos - az alap paramétereket
    {
        weaponAnim = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
        defaultYPos = playerCamera.transform.localPosition.y;
        defaultZRot = playerCamera.transform.localRotation.z;
        currentHealth = maxHealth;
        defaultFOV = playerCamera.fieldOfView;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame gugl transzi: framenként keni vágja.
    void Update() //gyakorlatilag az összes metódus csak akkor hívódik meg ha be van pipálva hogy true legyen. minden feltétel beteljesülésével átugrik maga a mozgás kezelésére. mindegyiket handler-nek neveztem el.
    {
        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();           
            if (CanJump)
            {
                HandleJump();
            }
            if (CanCrouch)
            {
                HandleCrouch();
            }
            if (CanUseHeadBob)
            {
                HandleHeadBob();
            }
            if (CanInteract)
            {
                HandleInteractionCheck();
                HandleInteractionInput();
            }
            if (CanZoom)
            {
                HandleZoom();
            }
            ApplyFinalMovements();
        }
        if (Input.GetKeyDown(flashKey) && CanLight) //ez az egyetlen kivétel ahol nem HandleFlashlight metódust csináltam.
        {

            if (isFlashOn == true)
            {
                light.gameObject.SetActive(false);
                isFlashOn = false;
                FindObjectOfType<AudioManager>().Play("flashOff");
                Debug.Log("flash is off");

            }
            else
            {
                light.gameObject.SetActive(true);
                isFlashOn = true;
                FindObjectOfType<AudioManager>().Play("flashOn");
                Debug.Log("flash is on");
            }
        }
        
        if (CantDrawCursor)
        {
            crosshairDefault.gameObject.GetComponent<Image>().enabled = false;
            crosshairUse.gameObject.GetComponent<Image>().enabled = false;
        }
        else if (crosshairUse.gameObject.GetComponent<Image>().enabled == false)
        {
            crosshairDefault.gameObject.GetComponent<Image>().enabled = true;
        }
    }


    //A zoomolás agya: ha lenyomjuk a zoomos gombot akkor berobban egy coroutine ami a zoomolásért felelős. HA ZOOMOLSZ NEM TUDSZ SPRINTELNI. Itt megy az animáció is meg majd a hang is később :)
    private void HandleZoom() 
    {
        if (Input.GetKeyDown(zoomKey))
        {
            weaponAnim.Play("ScopeIn", 0, 0.0f);
            CanSprint = false;
            if (zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                
                zoomRoutine = null;
            }
            zoomRoutine = StartCoroutine(ToggleZoom(true));
        }


        if (Input.GetKeyUp(zoomKey))
        {
            weaponAnim.Play("ScopeOut", 0, 0.0f);
            CanSprint = true;
            
            if (zoomRoutine != null)
            {              
                StopCoroutine(zoomRoutine);
                
                zoomRoutine = null;
            }
            zoomRoutine = StartCoroutine(ToggleZoom(false));
        }

       
        
    }
    //az alap mozgás agya: itt egy ternary operator segíségével eldöntöm hogy épp sprintelek, guggolok vagy sétálok-e. annak megfelelő sebességgel fogsz közlekedni.
    private void HandleMovementInput()
    {
        currentInput = new Vector2((IsCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (IsCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }
    //a nézelődés: egérbaszás megy ezerrel, nem tudom hogyan működik, de tudom, hogy ha zoomolsz akkor lassabban megy az egér
    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * (CantDrawCursor ? scopeLookSpeedY : lookSpeedY);
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * (CantDrawCursor ? scopeLookSpeedX : lookSpeedX), transform.rotation.eulerAngles.z);

    }
    //ha tudsz ugrani...akkor ugrassz... y irányba...
    private void HandleJump()
    {
        if (ShoudJump)
        {
            moveDirection.y = jumpForce;                    //szellemi érték: 100000000000000000000000huf +áfa
        }
    }
    //ha beindul a guggolás folyamat, akkor beindul a guggolás coroutine is
    private void HandleCrouch()
    {

        if (ShouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }
    // fejmozgás mozgás közben: ha nem vagyunk földön akkor off az egész, kilép a faszba. ellentétes esetben megnézi éppen milyen mozgásfajtát végzel, majd eldönti milyen sebességel fogsz headbangelni.
    //sinusba megy fel le és sinusba megy a rotálás is :)
    private void HandleHeadBob()
    {
        if (!characterController.isGrounded)
        {
            return;
        }

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {

            timerPos += Time.deltaTime * (IsCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timerPos) * (IsCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount), playerCamera.transform.localPosition.z);

            timerRot += Time.deltaTime * (IsCrouching ? crouchBobSpeedRot : IsSprinting ? sprintBobSpeedRot : walkBobSpeedRot);
            playerCamera.transform.rotation *= Quaternion.Euler(0, 0, defaultZRot + Mathf.Sin(timerRot) * (IsCrouching ? crouchBobAmountRot : IsSprinting ? sprintBobAmountRot : walkBobAmountRot));
        }
    }
    //AlkalmazSebzés: bedobjuk mennyi a sebzés és le is von annyit a hpból
    //ha nem sebez minket senki, akkor elkezdünk regelni. ha max a hp nem regelünk.

    private void ApplyDamage(float dmg)
    {
        currentHealth -= dmg;
        OnDamage?.Invoke(currentHealth);
        if (currentHealth <= 0)
        {
            KillPlayer();
        }
        else if (regenHealthRoutine != null)
        {
            StopCoroutine(regenHealthRoutine);
        }

        regenHealthRoutine = StartCoroutine(RegenHealt());
    }
    //egyelőre nincs kész, mert ugye itt kéne majd az újraéledés system. egyelőre csak elköszön a konzol ha nullán vagy. ugye milyen kedves konzol?
    private void KillPlayer()
    {
        currentHealth = 0;

        if (regenHealthRoutine != null)
        {
            StopCoroutine(regenHealthRoutine);
        }
        print("Bye");
    }
    //huha.. szóval ha a raycast a megadott határon belül találkozik gameobject-tel akkor nagy esellyel az Interactable ősosztályból az OnFocus() metódust fogja meghívni.
    //ha lenézünk róla akkor berobban az OnLoseFocus()
    private void HandleInteractionCheck()
    {
        if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint),out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.gameObject.layer == 10 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractable);
                if (currentInteractable && !Input.GetKey(zoomKey))
                {
                    currentInteractable.OnFocus();
                }
            }
        }
        else if (currentInteractable)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }
    //ez kezeli azt, hogy ha nyomunk az E betűt és a layer meg minden stimmel, akkor lehessen intearactolni a gameobjecttel... ugyanaz kb mint az eggyel fentebbi, csak itt már a layert is figyeli, mivel szabad interactolni (10-es layerrel))
    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactKey) && currentInteractable != null && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
        {
            currentInteractable.OnInteract();
        }
    }
    //ez felelős a gravitációért és a csúszásért a lejtőn, és persze hogy megmozduljon a karakter.
    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        if (WillSlideOnSlopse && IsSliding)
        {
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;
            CanJump = false;
        }
        else
        {
            CanJump = true;
        }


        characterController.Move(moveDirection * Time.deltaTime);
    }
    //a lankadás és állás közötti állapot.. amikor ez a coroutine elindul akkor a guggolás animációja fog menni. ha van felettünk valami vagy valaki, akkor nem tudsz felállni.
    private IEnumerator CrouchStand()
    {

        if (IsCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
        {
            yield break;
        }

        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = IsCrouching ? standHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = IsCrouching ? standCenter : crouchCenter;
        Vector3 currentCenter = characterController.center;

        while (timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        duringCrouchAnimation = false;
        IsCrouching = !IsCrouching;
    }
    //a zoomolás coroutinja... állígat mindent hogy jó legyen... leginkább a látószöget
    private IEnumerator ToggleZoom(bool isEnter)
    {
        float targetFOV = isEnter ? zoomFOV : defaultFOV;
        float startingFOV = playerCamera.fieldOfView;

        float timeElapsed = 0;
        while (timeElapsed < timeToZoom)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }      
        
        playerCamera.fieldOfView = targetFOV;
        zoomRoutine = null;
    }
    // a hp regelés szíve-lelke. vár egy kicsit és ha nem szopódik lejjebb a hápé, akkor regelünk szépen felfele
    private IEnumerator RegenHealt()
    {
        yield return new WaitForSeconds(regenTime);
        WaitForSeconds timeToWait = new WaitForSeconds(healthTimeIncrement);

        while (currentHealth < maxHealth)
        {
            currentHealth += heathValueIncrement;

            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            OnHeal?.Invoke(currentHealth);
            yield return timeToWait;
        }
        regenHealthRoutine = null;
    }
}
