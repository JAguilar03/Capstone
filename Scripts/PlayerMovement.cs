using System.Collections;
// using System.Collections.Generic;
// using System.Numerics;
using System;
// using System.Numerics;

// using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


using TMPro;

// This script handles the player's movement, rotation, and interaction with various game mechanics (e.g., health, punching, and grenade throwing).
// It includes functionality for handling both keyboard/mouse and gamepad input, as well as rotating the player to face the mouse or controller direction.
// The script also manages health, damage, healing, and the player animation states for walking, punching, and dying.
// Additionally, it handles pickups like health, ammo, weapons, and speed boosts, updating the player's state accordingly.


public enum AnimSet { Pistol, Rifle }

// [RequireComponent(typeof(CharacterController))]
// [RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    // player stats
    public float health = 100f;

    [SerializeField] private ScoreManager scoreManager;
    

    // public TextMeshProUGUI healthText;
    private bool isPunching = false;

    // movement vars
    public float moveSpeed = 5f; 
    public Rigidbody2D rb; 

    //Animation Data
    public AnimSet animSet = AnimSet.Pistol;
    public Animator animateLegs;
    public Animator animateBody;
    public Animator animateDeath;
    private Vector2 movement;
    private Vector2 playerVelocity;
    

    // rotation vars
    private Vector3 mouse_pos;
    private Vector3 object_pos;
    private float angle;
    private Vector2 aim;

    // Weapon Controller
    [SerializeField] private WeaponController weaponController;

    // Grenade prefab
    public GameObject grenadePrefab;

    //Controls
    // private CharacterController controller;
    // private PlayerControls playerControls;
    // private PlayerInput playerInput;

    public bool isGamepad = false; //Is the current control scheme gamepad?
    private float controllerDeadzone = 0.1f;

    private void Awake()
    {
        // controller = GetComponent<CharacterController>();
        // playerControls = new PlayerControls();
        // playerInput = GetComponent<PlayerInput>();

        // if (playerInput == null)
        // {
        //     Debug.LogWarning("PlayerInput was not assigned properly!");
        // }
    }

    private void OnEnable()
    {
        // playerControls.Enable();
        // KeybindManager.
    }

    private void OnDisable()
    {
        // playerControls.Disable();
    }

    void Update()
    {
        //TODO: Probably a bad idea to run this every frame during the death animation.
        //      It invokes a ton of functions to load the game over scene at once, which
        //      leads to weird behavior.
        if (health <= 0) {
            rb.velocity = Vector2.zero;
            transform.rotation = Quaternion.identity;
            // animate player death
            transform.GetComponent<CircleCollider2D>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(true);
            animateDeath.Play("Player_Death");

            // Save the score before transitioning to game over
            PlayerPrefs.SetInt("LastScore", scoreManager.GetCurrentScore());
            PlayerPrefs.Save();
            
            // Load GameOver scene after 2 seconds
            Invoke("LoadGameOver", 2f);
            return;
        }

        // handleInput();
        handleMovement();
        handleRotation();

        // // rotate player to face mouse
        mouse_pos = Input.mousePosition;
        mouse_pos.z = -(transform.position.z - Camera.main.transform.position.z);
        object_pos = Camera.main.WorldToScreenPoint(transform.position);
        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = mouse_pos.y - object_pos.y;
        angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
    }

    // void handleInput()
    // {
    //     // movement = playerControls.Controls.Movement.ReadValue<Vector2>();
    //     // movement = KeybindManager.instance.ReadValue<Vector2>();
    //     // aim = playerControls.Controls.Aim.ReadValue<Vector2>();
    //     // Debug.Log(movement);
    // }
    public void setMovement(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void setAim(InputAction.CallbackContext context)
    {
        aim = context.ReadValue<Vector2>();
    }

    public void punch(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !isPunching) {
            isPunching = true;
            animateBody.Play("Player_PunchPistol");
            Invoke("EndPunch", 1f);
        }
    }

    private void LoadGameOver()
    {
        //Delete the player character and HUD, otherwise they will persist into the gameOver scene
        Destroy(PlayerManager.playerInstance.gameObject);
        Destroy(HUDManager.hudInstance.gameObject);

        SceneManager.LoadScene("GameOver");
    }


    void handleMovement()
    {
        Vector2 input = new Vector2(movement.x, movement.y);
        rb.velocity = input.normalized * moveSpeed;

        // animate movement
        if (rb.velocity != Vector2.zero && !isPunching)
        {
            animateLegs.Play("Player_Legs_Walk");
            switch (animSet)
            {
                case AnimSet.Pistol:
                    animateBody.Play("Player_WalkPistol");
                    break;
                case AnimSet.Rifle:
                    animateBody.Play("Player_WalkRifle");
                    break;
            }
        }
        else if (!isPunching)
        {
            animateLegs.Play("Player_Legs_Idle");
            switch (animSet)
            {
                case AnimSet.Pistol:
                    animateBody.Play("Player_IdlePistol");
                    break;
                case AnimSet.Rifle:
                    animateBody.Play("Player_IdleRifle");
                    break;
            }
        }
    }

    void handleRotation()
    {

        if (Time.timeScale == 0) return;

        if (isGamepad)
        {
            //Rotate our player
            if (Math.Abs(aim.x) > controllerDeadzone || Math.Abs(aim.y) > controllerDeadzone)
            {
                Vector3 playerDirection = Vector3.up * aim.x + Vector3.left * aim.y;
                if (playerDirection.sqrMagnitude > 0.0f)
                {
                    Quaternion newRotation = Quaternion.LookRotation(Vector3.forward, playerDirection);
                    // transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, 1000 * Time.deltaTime);
                    transform.rotation = newRotation;
                    Debug.Log("newRotation: " + newRotation);
                }
            }
            
        }
        else
        {
            lookAtMouse();
        }
    }

    private void lookAtMouse()
    {
        //TODO: Rotate about z axis to face cursor
        // Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, lookPoint.y, transform.position.z);
        // transform.LookAt(heightCorrectedPoint);
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.right = mousePos - new Vector2(transform.position.x, transform.position.y);
    }

    // public void OnDeviceChange()
    // // public void OnDeviceChange(PlayerInput pin)
    // {
    //     PlayerInput pin = GetComponent<PlayerInput>();
    //     isGamepad = pin.currentControlScheme.Equals("Gamepad") ? true : false;
    // }

    void Start()
    {
        HUDManager.hudInstance.SetHealthDisplay(health);
        KeybindManager.instance.updateRefs();
        // UpdateHealthDisplay();
    }

    // private void UpdateHealthDisplay()
    // {
    //     healthText.text = "Health: " + health.ToString();
    // }

    public void TakeDamage(float damage)
    {
        health = Mathf.Max(0, health - damage);
        HUDManager.hudInstance.SetHealthDisplay(health);
        // UpdateHealthDisplay();
    }  

    public void HealDamage(float heal) {
        health = Mathf.Min(100, health + heal);
        HUDManager.hudInstance.SetHealthDisplay(health);
        // UpdateHealthDisplay();
    }

    private void EndPunch()
    {
        isPunching = false;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("EnemyAttack")) {
            // Debug.Log("Player Hit by Enemy");
            TakeDamage(other.transform.parent.gameObject.GetComponent<EnemyPathfinding2D>().attack);
        }
        if (other.CompareTag("Fireball")) {
            // Debug.Log("Player Hit by Fireball");
            TakeDamage(10f);
            Destroy(other.gameObject);
        }
        if (other.CompareTag("HealthPack")) {
            // Debug.Log("Player picked up Health Pack");
            HealDamage(25f);
            Destroy(other.gameObject);
        }
        if (other.CompareTag("AmmoPack")) {
            // Debug.Log("Player picked up Ammo Pack");
            weaponController.pickupAmmo();
            Destroy(other.gameObject);
        }
        if (other.CompareTag("WeaponPickup")) {
            // Debug.Log("Player picked up Ammo Pack");
            weaponController.addWeapon(other.GetComponent<WeaponPickup>().GetWeapon(), true);
            Destroy(other.gameObject);
        }
        if (other.CompareTag("SpeedPack")) {
            // Debug.Log("Player picked up Speed Pack");
            StartCoroutine(SpeedBoost(2f, 5f));
            Destroy(other.gameObject);
        }
    }

    //TODO: Actual throwing mechanics, right now grenade is just dropped at the player's feet
    public void ThrowGrenade(InputAction.CallbackContext context)
    {
        if (PauseManager.IsGamePaused) return;
        if (context.phase != InputActionPhase.Started) return;

        Instantiate<GameObject>(grenadePrefab, transform.position, transform.rotation);
    }

    IEnumerator SpeedBoost(float multiplier, float duration) {
        moveSpeed *= multiplier; 
        yield return new WaitForSeconds(duration);
        moveSpeed /= multiplier; 
    }
}
