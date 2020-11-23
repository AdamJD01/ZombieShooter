//main character of the game
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Steve : MonoBehaviour 
{
    //general variables 
    public float jumpForce = 6.0f; //default jumpForce value, can be changed in the editor

    private Rigidbody rigidSteve; //Steves body

    private int jumpSwitch; //as in flicking a switch on and off to jump
    private int checkPointTracker = 0; //tracks how many check points Steve has walked through

    private Vector3 savedPosition; //Steves position 

    private bool isGrounded; //for jumping

    public Animator steveAnimator; //is set to Steves animator in the editor

    public Transform shotDirection; //where the bullet came from, is set up in the editor

    public GameObject steveModel; //used to show Steve dying or celebrating, is set up in the editor
    private GameObject steveCorpse; //the Steve model when he is fainting
    private GameObject steveCamera; //the camera attached to Steve 
    public GameObject[] checkPoints; //all check points in current scene/level, is set up in the editor

    public LayerMask checkPointLayer; //layer of check points, is set up in the editor

    //AudioSource variables
    public AudioSource[] steveFootsteps; //is set to Steves footsteps audiosources in the editor
    public AudioSource steveJump; //is set to Steves jump audiosource in the editor
    public AudioSource steveLand; //is set to Steves land audiosource in the editor
    public AudioSource steveHurt; //is set to Steves hurt audiosource in the editor
    //public AudioSource steveDie;
    public AudioSource[] bloodSplats; //is set to the bloodsplats audiosources in the editor
    public AudioSource pickupAmmo; //is set to pickup ammo audiosource in the editor
    public AudioSource pickupMedkit; //is set to pickup medkit audiosource in the editor
    public AudioSource emptyGun; //is set to empty gun audiosource in the editor

    //ammo variables
    private int steveAmmo = 0; //the steveAmmo value Steve has when starting game
    public int addAmmo = 10; //the default addAmmo value, can be changed in editor
    private int maxSteveAmmo = 40; //the maxAmmo value Steve can carry
    public int ammoClip = 0; //the default ammoClip value, can be changed in editor
    private int maxAmmoClip = 10; //the default ammoClipMax value, can be changed in editor

    //health variables
    private int steveHealth = 0; //the steveHealth value Steve has when starting game
    public int minusHealth = 10; //the default minusHealth value, can be changed in editor
    public int addHealth = 20; //the default addHealth value, can be changed in editor
    private int maxSteveHealth = 100; //the maxHealth value of Steve
    public int numberOfLifes = 3; //the number of lifes Steve has when starting the game, can be changed in editor

    //Zombie variables
    private int bulletCounter = 0; //counts how many bullets have been shot at a zombie

    public GameObject zombieBlood; //blood that a zombie spawns when getting hit/killed, is set up in the editor

    //HUD variables
    public Slider healthBar; //used to display Steves health to the player, is set up in the editor

    public Text ammoAmount; //used to display the ammo currently in gun, is set up in the editor
    public Text ammoReserve; //used to display the ammo that can be put into gun, is set up in the editor

    public GameObject screenBlood; //used to show blood on screen when Steve gets hit, is set up in the editor
    public GameObject hudCanvas; //used to put Steves blood on actual HUD itself, is set up in the editor 
    public GameObject textGameOver; //used to show game over text when Steve wins, is set up in the editor

    private float canvasWidth; //width of the HUD
    private float canvasHeight; //height of the HUD

    private RectTransform rectTransform; //gameover text rect transform

    public Compass compassController; //compass that shows player where to go, is set up in the editor

    //debug variable
    public bool godMode = false; //used to make Steve immortal and have unlimited bullets for debugging, can be changed in the editor


    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined; //keep mouse inside the game
        Cursor.visible = false; //hide the cursor

        rigidSteve = GetComponent<Rigidbody>(); //give Steve a body

        steveCamera = GameObject.Find("Steve Camera"); //find the camera

        steveHealth = maxSteveHealth; //give Steve max health when starting the game
        steveAmmo = maxSteveAmmo; //give Steve max ammo when starting the game
        ammoClip = maxAmmoClip; //give Steve max ammo clip when starting the game

        savedPosition = transform.localPosition; //save Steves position when first loading the game

        healthBar.value = steveHealth; //set the healthbar to Steves health

        ammoAmount.text = ammoClip.ToString(); //set text to the guns ammo
        ammoReserve.text = steveAmmo.ToString(); //set text to the reserve ammo

        canvasWidth = hudCanvas.GetComponent<RectTransform>().rect.width; //get width of the HUD
        canvasHeight = hudCanvas.GetComponent<RectTransform>().rect.height; //get height of the HUD

        rectTransform = textGameOver.GetComponent<RectTransform>(); //find rect transform of the gameover text 

        compassController.compassTarget = checkPoints[0]; //find the first compass target
    }

    void Update()
    {
        CheckMouseCursor(); //check if cursor should be off or on in build

        //don't do the below code if Steve has fainted, is dead or has won
        if (GameStats.hasFainted || GameStats.isDead || GameStats.wonGame)
        {
            return;
        }

        //jumping
        float jumping = Input.GetAxis("Jump") + jumpForce; //jumping input
        jumping += Time.deltaTime;

        if ((Input.GetAxis("Jump") != 0) && isGrounded)
        {
            if (jumpSwitch == 0) //"switch" is off
            {
                jumpSwitch = 1; //"switch" is on
                rigidSteve.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); //move Steve upwards
                PlayJumpingSound();
                //CancelInvoke("PlayRandomFootsteps");
            }

            //"switch" is on so turn it back off
            if (jumpSwitch > 0)
            {
                jumpSwitch--;
                isGrounded = false;
            }
        }
    }

    void FixedUpdate()
    {
        //don't do the below code if Steve has fainted, is dead or has won
        if (GameStats.hasFainted || GameStats.isDead || GameStats.wonGame)
        {
            return;
        }

        PlayStevesAnimationsAndFootstepSounds(); //decide which animation and/or footstep sounds to play

        //movement 
        float speed = 0.1f; 
        float moving = Input.GetAxis("Vertical") * speed; //vertical inputs
        float strafe = Input.GetAxis("Horizontal") * speed; //horizontal inputs
        //moving *= Time.deltaTime;
        //strafe *= Time.deltaTime;

        transform.Translate(strafe, 0, moving);

        CheckMouseCursor(); //check if cursor should be off or on in editor
    }

    void OnCollisionEnter(Collision steveCollision)
    {
        //don't do the below code if Steve has fainted, is dead or has won
        if (GameStats.hasFainted || GameStats.isDead || GameStats.wonGame)
        {
            return;
        }

        //same as jumpSwitch but detects falling instead
        int falling;
        falling = jumpSwitch;

        isGrounded = true; //on the ground

        //play the landed sound if falling has finished (0 means Steve has stopped falling)
        if (falling == 0)
        {
            PlayLandedSound();
        }

        //moved here so this happens after falling as well as jumping
        if (steveAnimator.GetBool("walking"))
        {
            InvokeRepeating("PlayRandomFootsteps", 0, 0.4f);
        }

        //take health off Steve if hitting lava
        if (steveCollision.gameObject.tag == "Lava")
        {
            SteveTakeDamage(10);
        }

        //pickup ammo if Steve is low on bullets
        else if ((steveCollision.gameObject.tag == "Ammo") && (steveAmmo < maxSteveAmmo))
        {
            Destroy(steveCollision.gameObject);
            PlayAmmoPickupSound();

            steveAmmo = Mathf.Clamp(steveAmmo + addAmmo, 0, maxSteveAmmo);
            ammoReserve.text = steveAmmo.ToString();
            //Debug.Log("Current ammo = " + steveAmmo);
        }

        //pickup health if Steve is hurt
        else if ((steveCollision.gameObject.tag == "MedKit") && (steveHealth < maxSteveHealth))
        {
            Destroy(steveCollision.gameObject);
            PlayMedkitPickupSound();

            steveHealth = Mathf.Clamp(steveHealth + addHealth, 0, maxSteveHealth);
            healthBar.value = steveHealth;
            //Debug.Log("Current health = " + steveHealth);
        }
    }

    void OnCollisionExit()
    {
        //don't do the below code if Steve has fainted, is dead or has won
        if (GameStats.hasFainted || GameStats.isDead || GameStats.wonGame)
        {
            return;
        }

        isGrounded = false; //off the ground

        CancelInvoke("PlayRandomFootsteps"); //moved here so this happens when falling as well as jumping
    }

    void OnTriggerEnter(Collider steveCollider)
    {
        //don't do the below code if Steve has fainted, is dead or has won
        if (GameStats.hasFainted || GameStats.isDead || GameStats.wonGame)
        {
            return;
        }

        if (steveCollider.gameObject.tag == "Home")
        {
            //set up the victory celebration, show game over text and end the game
            Vector3 victory = new Vector3(transform.localPosition.x, Terrain.activeTerrain.SampleHeight(transform.localPosition), transform.localPosition.z);
            GameObject steve = Instantiate(steveModel, victory, transform.localRotation);
            steve.GetComponent<AudioSource>().Play();
            GameObject text = Instantiate(textGameOver);
            text.transform.SetParent(hudCanvas.transform);
            text.transform.localPosition = new Vector3(rectTransform.position.x, rectTransform.position.y, 0);
            steve.GetComponent<Animator>().SetTrigger("Dance");
            GameStats.wonGame = true;
            Destroy(gameObject);
        }

        if(steveCollider.gameObject.tag == "CheckPoint")
        {
            //save Steves new position if he is at a check point and update the compass
            savedPosition = transform.localPosition; 

            if (steveCollider.gameObject == checkPoints[checkPointTracker])
            {
                checkPointTracker++;
                compassController.compassTarget = checkPoints[checkPointTracker];
            }
        }
    }

    protected void PlayStevesAnimationsAndFootstepSounds()
    {
        //play walking animations and footstep sounds when moving or strafing
        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            if (!steveAnimator.GetBool("walking"))
            {
                steveAnimator.SetBool("walking", true);
                InvokeRepeating("PlayRandomFootsteps", 0, 0.4f);
            }
        }

        //play idle animations and stop footsteep sounds when not moving or strafing
        else if (steveAnimator.GetBool("walking"))
        {
            steveAnimator.SetBool("walking", false);
            CancelInvoke("PlayRandomFootsteps");
        }

        //go into the holding gun animation if pressing a custom HoldWeapon input (right mouse or alt) and is not already holding the gun
        if ((Input.GetButtonDown("HoldWeapon")) && (!steveAnimator.GetBool("arm")))
        {
            steveAnimator.SetBool("arm", true);
        }

        //go back to the idle animation if pressing a custom HoldWeapon input (right mouse or alt) and is holding gun
        else if ((Input.GetButtonDown("HoldWeapon")) && (steveAnimator.GetBool("arm")))
        {
            steveAnimator.SetBool("arm", false);
        }

        //go into the fire gun animation if pressing a Fire1 input (left mouse or ctrl), is holding gun and is not currently shooting or reloadng
        else if (Input.GetButtonDown("Fire1") && (steveAnimator.GetBool("arm")) && (!steveAnimator.GetBool("fire")) && GameStats.canShoot)
        {
            //shoot if Steve has ammo
            if (ammoClip > 0)
            {
                steveAnimator.SetTrigger("fire");
                HitZombie();

                //reduce ammo if not in god mode
                if (!godMode)
                {
                    ammoClip--;
                    ammoAmount.text = ammoClip.ToString();
                    GameStats.canShoot = false;
                }
            }
            
            //don't shoot if Steve has no ammo
            else
            {
                GameStats.canShoot = false;
                PlayEmptyGunSound();
            }

            //Debug.Log("Ammo left in clip = " + ammoClip);
        }

        //go into the reloading animation if pressing the reload key (r), has an ammo clip, is not currently reloading and is holding gun
        else if ((Input.GetKeyDown(KeyCode.R)) && (steveAnimator.GetBool("arm")))
        {         
            int amount = maxAmmoClip - ammoClip;
            int available = amount < steveAmmo ? amount : steveAmmo;
            steveAmmo -= available;
            ammoClip += available;
            ammoReserve.text = steveAmmo.ToString();

            if (available > 0)
            {
                steveAnimator.SetTrigger("reload");
                ammoAmount.text = ammoClip.ToString();
                GameStats.canShoot = false;
                //Debug.Log("Ammo Left = " + steveAmmo);
                //Debug.Log("Ammo in clip = " + ammoClip);
            }
        }
    }

    protected void PlayRandomFootsteps()
    {
        //set up the random footstep
        AudioSource randomFootstep = new AudioSource();
        int footstep = Random.Range(1, steveFootsteps.Length);

        //play the random footstep and skip the footstep that was last used
        randomFootstep = steveFootsteps[footstep];
        randomFootstep.Play();
        steveFootsteps[footstep] = steveFootsteps[0];
        steveFootsteps[0] = randomFootstep;
    }

    protected void PlayJumpingSound()
    {
        steveJump.Play(); //play the jumping sound
    }

    protected void PlayLandedSound()
    {
        steveLand.Play(); //play the landed sound

        /*
        if (steveAnimator.GetBool("walking"))
        {
            InvokeRepeating("PlayRandomFootsteps", 0, 0.4f);
        }
        */
    }

    protected void PlayHurtSound()
    {
        //stop playing the sound if health is under 1
        if (steveHealth <= 0)
        {
            return;
        }

        steveHurt.Play(); //play the hurt sound
    }

    /*
    public void PlayDieSound()
    {
        steveDie.Play(); 
    }
    */

    protected void PlayRandomBloodSplatSound()
    {
        //stop playing the sound if health is under 1
        if (steveHealth <= 0)
        {
            return;
        }

        //set up the random bloodsplat
        AudioSource randomBloodSplat = new AudioSource();
        int blood = Random.Range(1, bloodSplats.Length);

        //play the random bloodsplat and skip the bloodsplat that was last used
        randomBloodSplat = bloodSplats[blood];
        randomBloodSplat.Play();
        bloodSplats[blood] = bloodSplats[0];
        bloodSplats[0] = randomBloodSplat;
    }

    protected void PlayAmmoPickupSound()
    {
        pickupAmmo.Play(); //play the picked up ammo sound
    }

    protected void PlayMedkitPickupSound()
    {
        pickupMedkit.Play(); //play the picked up medkit sound
    }

    protected void PlayEmptyGunSound()
    {
        emptyGun.Play(); //play the empty gun sound
    }

    protected void HitZombie()
    {
        RaycastHit hit;

        //hurt Zombie if the gun has shot at them, update bullet counter and spawn blood if not a check point
        if (Physics.Raycast(shotDirection.position, shotDirection.forward, out hit, 200, ~checkPointLayer))
        {
            GameObject zombie = hit.collider.gameObject;

            if (zombie.tag == "Zombie")
            {
                GameObject blood = Instantiate(zombieBlood, hit.point, Quaternion.identity);
                blood.transform.LookAt(transform.localPosition);
                Destroy(blood, 0.5f);

                bulletCounter++;

                //reset bullet counter and play a random death sequence
                if (bulletCounter == zombie.GetComponent<Zombie>().shotsToKill)
                {
                    bulletCounter = 0;

                    if (Random.Range(0, 10) < 5)
                    {
                        GameObject ragdoll = zombie.GetComponent<Zombie>().zombieRagdoll;
                        GameObject copy = Instantiate(ragdoll, zombie.transform.localPosition, zombie.transform.localRotation);
                        copy.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(shotDirection.forward * 10000);
                        Destroy(zombie);
                    }

                    else
                    {
                        zombie.GetComponent<Zombie>().KillZombieAnimation();
                    }
                }
            }
        }
    }

    public void SteveTakeDamage(float damageAmount)
    {
        //don't do the below code if Steve has fainted
        if (GameStats.hasFainted)
        {
            return;
        }

        //don't do the below code if in god mode
        if (godMode)
        {
            return;
        }

        if (steveHealth <= 0 )
        {
            //set up Steve fainting and go back to saved position if all the lifes are not used up
            if (numberOfLifes > 0)
            {
                GameStats.hasFainted = true;
                Vector3 dead = new Vector3(transform.localPosition.x, Terrain.activeTerrain.SampleHeight(transform.localPosition), transform.localPosition.z);
                steveCorpse = Instantiate(steveModel, dead, transform.localRotation);
                steveCorpse.GetComponent<Animator>().SetTrigger("Death");
                steveCorpse.GetComponent<AudioSource>().enabled = false;
                steveCamera.SetActive(false);
                Invoke("RespawnSteve", 10);
            }

            //otherwise set up Steve dying animation and end the game if there are no more lifes
            else
            {
                GameStats.isDead = true;
                Vector3 dead = new Vector3(transform.localPosition.x, Terrain.activeTerrain.SampleHeight(transform.localPosition), transform.localPosition.z);
                GameObject corpse = Instantiate(steveModel, dead, transform.localRotation);
                healthBar.value = 0;
                //PlayDieSound();
                corpse.GetComponent<Animator>().SetTrigger("Death");
                Destroy(gameObject);
            }
        }

        //take health off Steve if being hit by a Zombie, create random blood on screen and play the appropriate sounds
        steveHealth = (int)Mathf.Clamp(steveHealth - damageAmount, 0, maxSteveHealth);
        healthBar.value = steveHealth;

        GameObject splatter = Instantiate(screenBlood);
        splatter.transform.SetParent(hudCanvas.transform);
        float scale = Random.Range(0.3f, 1.5f);
        splatter.transform.localScale = new Vector3 (scale, scale, 1);
        splatter.transform.position = new Vector3 (Random.Range(0, canvasWidth), Random.Range(0, canvasHeight), 0);
        Destroy(splatter, 2.5f);

        PlayHurtSound();
        PlayRandomBloodSplatSound();
        //Debug.Log("Steve Health = " + steveHealth);
    }

    protected void RespawnSteve()
    {
        //clean up level, give Steve his health back, spawn him at the saved position and take off a life
        Destroy(steveCorpse);
        steveCamera.SetActive(true);
        GameStats.hasFainted = false;
        steveHealth = maxSteveHealth;
        healthBar.value = maxSteveHealth;
        transform.localPosition = savedPosition;
        numberOfLifes -= 1;
        //Debug.Log("Lifes left = " + numberOfLifes);
    }

    protected void CheckMouseCursor()
    {
        //show cursor if pressed the escape key
        if ((Input.GetAxis("Cancel") != 0) && (Cursor.lockState == CursorLockMode.Confined))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        //hide cursor if clicked back on the game and not pointing over main menu button
        else if ((Input.GetAxis("Fire1") != 0) && (Cursor.visible && Cursor.lockState == CursorLockMode.None) && !EventSystem.current.IsPointerOverGameObject())
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
    }
}
