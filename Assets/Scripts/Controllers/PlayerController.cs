using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.InputSystem.Interactions;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int playerHealth;
    [SerializeField] private float playerSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 100.0f;
    [SerializeField] private GameObject singleShoot;
    [SerializeField] private float shootSpeed;
    [SerializeField] private float shootDelay;
    [SerializeField] private GameObject cannonPivot;
    [SerializeField] private int tripleShootQuant = 99;
    
    private Rigidbody2D _playerRb;
    private BoxCollider2D _playerCollider;
    private Animator _playerAnimator;
    
    private float moveInput;
    private float rotateInput;
    private bool rotateClockwise;
    private bool rotateCounterClockwise;
    private bool isRotating;
    public bool canFire = true;
    private void Start()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        _playerCollider = GetComponent<BoxCollider2D>();
        _playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
        if (isRotating)
        {
            Rotation();    
        }
    }

    void Rotation()
    {
        if (rotateClockwise)
        {
            transform.Rotate(0, 0, -rotateInput * rotationSpeed * Time.deltaTime);
        }
        else if (rotateCounterClockwise)
        {
            transform.Rotate(0, 0, rotateInput * rotationSpeed * Time.deltaTime);
        }
    }
    
    void Move()
    {
        Vector2 forward = transform.up;
        _playerRb.velocity = -forward * moveInput * playerSpeed;
    }

    public void OnFoward(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();
    }

    public void OnRotateClockwise(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case (InputActionPhase.Performed):
                isRotating = true;
                rotateInput = context.ReadValue<float>();
                rotateClockwise = true;
                rotateCounterClockwise = false;
                break;
            
            case (InputActionPhase.Canceled):
                isRotating = false;
                break;
        }
    }

    public void OnRotateCounterClockwise(InputAction.CallbackContext context)
    {
        isRotating = true;
        rotateInput = context.ReadValue<float>();
        
        rotateCounterClockwise = true;
        rotateClockwise = false;
    }

    public void OnFireOneShot(InputAction.CallbackContext context)
    {
        if (canFire)
        {
            StartCoroutine(ShootOneBullet());  
        }
    }

    public void OnFireTripleShot(InputAction.CallbackContext context)
    {
        if (canFire && tripleShootQuant > 0)
        {
            StartCoroutine(ShootTripleBullets());
        }
    }
    
    IEnumerator ShootOneBullet()
    {
        canFire = false;
        Debug.Log("Atira");
        
        float playerRotation = transform.rotation.eulerAngles.z;
        Vector2 bulletDirection = Quaternion.Euler(0, 0, playerRotation) * Vector2.down;
        
        GameObject shoot = Instantiate(singleShoot, transform.position, Quaternion.identity);
        Rigidbody2D rb = shoot.GetComponent<Rigidbody2D>();
        rb.velocity = bulletDirection * shootSpeed;
        yield return new WaitForSecondsRealtime(shootDelay);
        canFire = true;
    }
    
    IEnumerator ShootTripleBullets()
    {
        canFire = false;
        tripleShootQuant--;
        Debug.Log("Atira 3");
        
        float cannonRotation = cannonPivot.transform.rotation.eulerAngles.z;
        
        Vector2 centralDirection = Quaternion.Euler(0, 0, cannonRotation) * Vector2.right;
        Vector2 leftDirection = Quaternion.Euler(0, 0, cannonRotation) * new Vector2(1, -1);
        Vector2 rightDirection = Quaternion.Euler(0, 0, cannonRotation) * new Vector2(1, 1);

        GameObject shootLeft = Instantiate(singleShoot, cannonPivot.transform.position, Quaternion.identity);
        GameObject shootCentral = Instantiate(singleShoot, cannonPivot.transform.position, Quaternion.identity);
        GameObject shootRight = Instantiate(singleShoot, cannonPivot.transform.position, Quaternion.identity);

        Rigidbody2D rbCentral = shootCentral.GetComponent<Rigidbody2D>();
        rbCentral.velocity = centralDirection * shootSpeed;

        Rigidbody2D rbLeft = shootLeft.GetComponent<Rigidbody2D>();
        rbLeft.velocity = leftDirection * shootSpeed;

        Rigidbody2D rbRight = shootRight.GetComponent<Rigidbody2D>();
        rbRight.velocity = rightDirection * shootSpeed;

        yield return new WaitForSecondsRealtime(shootDelay);
        canFire = true;
    }


    public void TakeDamage(int bulletDamage)
    {
        playerHealth -= bulletDamage;
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            Die();
        }
    }

    public void Die()
    {
        StartCoroutine(WaitForAnimationToEnd());
    }
        
    IEnumerator WaitForAnimationToEnd()
    {
        GetComponent<PlayerInput>().DeactivateInput();
        _playerAnimator.SetBool("IsDead", true);
        yield return new WaitForSeconds(_playerAnimator.GetCurrentAnimatorStateInfo(0).length);
        Debug.Log("Morreu");
    }
}
