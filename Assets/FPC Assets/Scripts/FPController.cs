using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FPController : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] float movementSpeed = 10f;
    // [SerializeField] float sprintSpeed = 20f;
    [SerializeField] float jumpForce = 300f;
    [Range(0, 10)][SerializeField] float sensitivity = 1f;
    float maxSensitivity = 10f;
    [SerializeField] float bulletForce = 800f;


    [Header("References")]
    [SerializeField] Camera fpsCamera;
    [SerializeField] Animator anim;
    [SerializeField] GameObject fullBodyModel;
    [SerializeField] AudioSource[] footSteps;
    [SerializeField] AudioSource jump;
    [SerializeField] AudioSource land;
    [SerializeField] AudioSource ammokitAudio;
    [SerializeField] AudioSource medkitAudio;
    [SerializeField] AudioSource outOfAmmo;
    [SerializeField] AudioSource dealth;
    [SerializeField] AudioSource reload;
    [SerializeField] LevelController levelController;


    [Header("UI References")]
    [SerializeField] GameObject aim;
    [SerializeField] Text ammunationText;
    [SerializeField] GameObject[] ammoClips;
    [SerializeField] GameObject LevelCompleteMenu;
    [SerializeField] GameObject WaitMenu;
    [SerializeField] Text waitMenuLevelText;
    [SerializeField] Text zombiesLeftText;
    [SerializeField] Image healthBar;
    [SerializeField] Text healthText;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] Slider sensitivitySlider;



    Rigidbody rb;
    CapsuleCollider fpsCollider;
    Quaternion camRotation, fpsRotation;
    bool cursorIsLocked = true;
    float x, z;
    bool isDead = false;
    bool playingWalking = false;
    bool previouslyGrounded = true;
    public MapLocation finishLine;
    GameObject temp;
    public bool canShoot = true;
    bool grounded;


    // Inventory
    int ammo = 100;
    int maxAmmo = 100;
    int ammoClip = 10;
    int maxAmmoClip = 10;
    float health = 100;
    float maxHealth = 100;


    private void Awake()
    {
        Time.timeScale = 1;
        sensitivity = GameStats.sensitivity;
        sensitivitySlider.value = sensitivity;
        LockCursor(true);
    }


    void Start()
    {
        GameStats.bulletForce = bulletForce;
        rb = GetComponent<Rigidbody>();
        fpsCollider = GetComponent<CapsuleCollider>();

        camRotation = fpsCamera.transform.localRotation;
        fpsRotation = this.transform.localRotation;


        RefreshDisplay();

        if (GameStats.currLevel == 1)
        {
            Invoke("ZomBieCanAttack", 5f);
        }
    }


    // Interval between 2 Fixed Update Functions is not constant...
    void Update()
    {
        // FPC Jump
        grounded = IsOnGround();
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(0, jumpForce, 0);
            jump.Play();
            if (anim.GetBool("WalkWithWeapon"))
            {
                CancelInvoke("RandomFootsteps");
                playingWalking = false;
            }
        }
        else if (!previouslyGrounded && grounded)
        {
            land.Play();
        }
        previouslyGrounded = grounded;


        // Gets Weapon
        if (Input.GetKeyDown(KeyCode.F))
            anim.SetBool("Weapon", !anim.GetBool("Weapon"));

        // Enable Aim
        if (anim.GetBool("Weapon"))
            aim.SetActive(true);
        else
            aim.SetActive(false);

        // Fire 
        if (Input.GetKeyDown(KeyCode.Mouse0) && anim.GetBool("Weapon"))
        {
            if (ammoClip > 0)
            {
                if (canShoot)
                {
                    canShoot = false;
                    anim.SetTrigger("Fire");
                    Shoot();
                }
            }
            else
            {
                outOfAmmo.Play();
            }
            Debug.Log("AmmoClip: " + ammoClip);
            Debug.Log("Ammo: " + ammo);
        }

        // Reload Gun
        if (Input.GetKeyDown(KeyCode.R) && anim.GetBool("Weapon"))
        {
            canShoot = false;
            Invoke("ToggleCanShoot", 3.1f);
            anim.SetTrigger("Reload");

            reload.Play();
        }

        // Walking with Weapon
        if (x != 0 || z != 0)
        {
            if (!anim.GetBool("WalkWithWeapon"))
            {
                anim.SetBool("WalkWithWeapon", true);
                if (!playingWalking)
                    InvokeRepeating("RandomFootsteps", 0, 0.4f);
            }
        }
        else if (anim.GetBool("WalkWithWeapon"))
        {
            anim.SetBool("WalkWithWeapon", false);
            CancelInvoke("RandomFootsteps");
            playingWalking = false;
        }

        // // Check if dead
        // if (health <= 0 && !isDead)
        // {
        //     isDead = true;
        //     dealth.Play();
        // }

        HealthBarFiller();
    }



    // Interval between 2 Fixed Update Functions is always constant...
    private void FixedUpdate()
    {
        // Moving FPC
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        // if (Input.GetKey(KeyCode.LeftShift))
        // {
        //     transform.Translate(x * sprintSpeed * Time.deltaTime, 0, z * sprintSpeed * Time.deltaTime, Space.Self);
        // }
        // else
        // {
        transform.Translate(x * movementSpeed * Time.deltaTime, 0, z * movementSpeed * Time.deltaTime, Space.Self);
        // }


        // Rotating FPS with mouse
        // Mouse ko horizonal move kroge to FPS Local Y-Axis pr rotate hoga.
        float yAxisRot = Input.GetAxis("Mouse X");
        // Mouse ko Vertical move kroge to Camera Local X-Axis pr rotate hoga.
        float xAxisRot = Input.GetAxis("Mouse Y");

        camRotation *= Quaternion.Euler(-xAxisRot * sensitivity, 0, 0);
        camRotation = ClampRotationXAxis(camRotation);
        fpsRotation *= Quaternion.Euler(0, yAxisRot * sensitivity, 0);

        fpsCamera.transform.localRotation = camRotation;
        this.transform.localRotation = fpsRotation;


        // Toggle Cursor
        ToggleCursor();
    }



    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ammo" && ammo < maxAmmo)
        {
            ammo = Mathf.Clamp(ammo + 10, 0, maxAmmo);
            ammokitAudio.Play();
            Debug.Log("Ammo: " + ammo);
            Destroy(other.gameObject);
            RefreshDisplay();
        }
        else if (other.gameObject.tag == "Med" && health < maxHealth)
        {
            health = Mathf.Clamp(health + 25, 0, maxHealth);
            medkitAudio.Play();
            Debug.Log("Health: " + health);
            Destroy(other.gameObject);
            RefreshDisplay();
        }
        else if (other.gameObject.tag == "Danger")
        {
            InvokeRepeating("Danger", 0.5f, 1f);
            dealth.Play();
        }

        if (IsOnGround())
        {
            if (anim.GetBool("WalkWithWeapon") && !playingWalking)
                InvokeRepeating("RandomFootsteps", 0, 0.4f);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Danger")
        {
            CancelInvoke("Danger");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "FinishLine")
        {
            GameoverWith("Dance");
            GameStats.zombieCanAttack = false;
        }
    }



    public void Danger()
    {
        health = Mathf.Clamp(health - 5, 0, maxHealth);
        // Check if dead
        if (health <= 0 && !isDead)
        {
            isDead = true;
            dealth.Play();
        }
        Debug.Log("Health: " + health);
    }



    // Casting a Sphere from the position of FPS...
    // Sphere is of radius = FPS(Capsule Radius)
    // Sphere is casted downwards
    // hit will store all the information like objects that sphere hitted.
    // next is the distance from capsule position till which the sphere's center will go...
    // Physics.SphereCast returns true when it collides with some other collider otherwise it returns false...
    private bool IsOnGround()
    {
        RaycastHit hit;
        return Physics.SphereCast(transform.position, fpsCollider.radius, Vector3.down, out hit,
                                 fpsCollider.height / 2f - fpsCollider.radius + 0.05f);

    }


    // Jb mouse ko vertical move krenge to horizontal plane se 90 degree angle se jyada move ni kr paaenge camera.
    Quaternion ClampRotationXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, -90, 90);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }



    private void ToggleCursor()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && cursorIsLocked)
        {
            LockCursor(false);
            Pause(true);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && !cursorIsLocked)
        {
            LockCursor(true);
        }
    }
    public void LockCursor(bool action)
    {
        cursorIsLocked = action;
        if (cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }



    // Play Random Walking Sound
    public void RandomFootsteps()
    {
        int idx = Random.Range(1, footSteps.Length);
        AudioSource temp = new AudioSource();

        temp = footSteps[idx];
        temp.Play();
        footSteps[idx] = footSteps[0];
        footSteps[0] = temp;
        playingWalking = true;
    }


    private void Shoot()
    {
        Invoke("ToggleCanShoot", 0.73f);
        ammoClip--;
        RefreshDisplay();
        RaycastHit hitInfo;
        if (Physics.Raycast(fpsCamera.gameObject.transform.position, fpsCamera.gameObject.transform.forward, out hitInfo, 200))
        {
            GameObject shotObj = hitInfo.collider.gameObject;
            if (shotObj.tag == "Zombie")
            {
                zombiesLeftText.text = Mathf.Max(0, --GameStats.totalZombiesInCurrentLevel).ToString("00");
                ZombieController2 zc2 = shotObj.GetComponent<ZombieController2>();
                zc2.KillSelf();
            }
        }
    }


    public void Reload()
    {
        // Reloading ammoclip
        int ammoNeeded = maxAmmoClip - ammoClip;
        int ammoAvailable = Mathf.Min(ammoNeeded, ammo);
        ammoClip += ammoAvailable;
        ammo -= ammoAvailable;
        RefreshDisplay();
    }


    private void ToggleCanShoot()
    {
        canShoot = true;
    }

    public void TakeDamage(float damageAmount)
    {
        health = (int)Mathf.Clamp((float)health - damageAmount, 0, maxHealth);
        RefreshDisplay();

        if (health <= 0)
        {
            GameoverWith("Death");
        }
        // Debug.Log("Health: " + health);
    }


    private void GameoverWith(string action)
    {
        CancelInvoke("RandomFootsteps");
        if (action == "Dance")
        {
            LevelCompleteMenu.SetActive(true);
            CancelInvoke("RandomFootsteps");
        }
        LockCursor(false);

        GameStats.gameOver = true;

        aim.SetActive(false);

        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        temp = Instantiate(fullBodyModel, pos, transform.rotation);
        temp.GetComponent<Animator>().SetTrigger(action);

        gameObject.SetActive(false);
    }


    private void RefreshDisplay()
    {
        healthText.text = health.ToString("00");
        // ammoClipText.text = "AmmoClip : " + ammoClip;
        for (int i = 0; i < ammoClips.Length; i++)
        {
            if ((i + 1) <= ammoClip)
                ammoClips[i].SetActive(true);

            else
                ammoClips[i].SetActive(false);
        }
        ammunationText.text = ammo.ToString();
    }

    private void HealthBarFiller()
    {
        float lerpSpeed = 5 * Time.deltaTime;
        float from = Mathf.Min(healthBar.fillAmount, health / 100f);
        float to = Mathf.Max(healthBar.fillAmount, health / 100f);
        healthBar.fillAmount = Mathf.Lerp(from, to, lerpSpeed);
    }

    private void ResetHealthAndAmmo()
    {
        health = maxHealth;
        ammoClip = maxAmmoClip;
        ammo = maxAmmo;
        RefreshDisplay();
    }

    public void NextLevel()
    {
        levelController.MoveToNextLevel();
        Destroy(temp);
        WaitMenu.SetActive(true);
        waitMenuLevelText.text = "Level " + GameStats.currLevel.ToString();
        ResetHealthAndAmmo();
        Invoke("WaitUi", Random.Range(2f, 3f));
        LevelCompleteMenu.SetActive(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        LevelCompleteMenu.SetActive(false);
        PauseMenu.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }

    public void WaitUi()
    {
        WaitMenu.SetActive(false);
        Invoke("ZomBieCanAttack", 5f);
    }

    public void ZomBieCanAttack()
    {
        GameStats.zombieCanAttack = true;
    }


    public void Pause(bool action)
    {
        if (action)
        {
            PauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            PauseMenu.SetActive(false);
            LockCursor(true);
            Time.timeScale = 1;
        }
    }


    public void SensitivityChange()
    {
        sensitivity = sensitivitySlider.value;
    }

    public void SettingsBackButton()
    {
        if (GameStats.gameOver)
        {
            LevelCompleteMenu.SetActive(true);
        }
        else
        {
            PauseMenu.SetActive(true);
        }
    }
}
