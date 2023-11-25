using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Utils;

public class PlayerController : MonoBehaviour
{
    public int playerHealth;
    public int maxHealth;
    [SerializeField] private float playerSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 100.0f;
    [SerializeField] private GameObject singleShoot;
    [SerializeField] private float shootSpeed;
    [SerializeField] private float shootDelay;
    [SerializeField] private GameObject cannonPivot;
    [SerializeField] private HealthBar healthBar;
    
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float duration;
    private Material originalMaterial;
    private Coroutine flashRoutine;
    
    private Rigidbody2D playerRb;
    private Animator playerAnimator;
    private SpriteRenderer spriteRenderer;
    
    private float moveInput;
    private float rotateInput;
    private bool rotateClockwise;
    private bool rotateCounterClockwise;
    private bool isRotating;
    private bool canFire = true;
    
    private void Start()
    {
        maxHealth = playerHealth;
        playerRb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
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
        playerRb.velocity = -forward * moveInput * playerSpeed;
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
        if (canFire)
        {
            StartCoroutine(ShootTripleBullets());
        }
    }
    
    IEnumerator ShootOneBullet()
    {
        canFire = false;
        
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
        
        Vector2 centralDirectionParallel = Quaternion.Euler(0, 0, cannonRotation) * Vector2.left;
        Vector2 leftDirectionParallel = Quaternion.Euler(0, 0, cannonRotation) * new Vector2(-1, 1);
        Vector2 rightDirectionParallel = Quaternion.Euler(0, 0, cannonRotation) * new Vector2(-1, -1);

        GameObject shootLeftParallel = Instantiate(singleShoot, cannonPivot.transform.position, Quaternion.identity);
        GameObject shootCentralParallel = Instantiate(singleShoot, cannonPivot.transform.position, Quaternion.identity);
        GameObject shootRightParallel = Instantiate(singleShoot, cannonPivot.transform.position, Quaternion.identity);

        Rigidbody2D rbCentralParallel = shootCentralParallel.GetComponent<Rigidbody2D>();
        rbCentralParallel.velocity = centralDirectionParallel * shootSpeed;

        Rigidbody2D rbLeftParallel = shootLeftParallel.GetComponent<Rigidbody2D>();
        rbLeftParallel.velocity = leftDirectionParallel * shootSpeed;

        Rigidbody2D rbRightParallel = shootRightParallel.GetComponent<Rigidbody2D>();
        rbRightParallel.velocity = rightDirectionParallel * shootSpeed;

        yield return new WaitForSecondsRealtime(shootDelay);
        canFire = true;
    }


    public void TakeDamage(int bulletDamage)
    {
        StartCoroutine(ShootAnimation());
        Flash();
        playerHealth -= bulletDamage;
        if (playerHealth <= maxHealth / 2)
        {
            playerAnimator.SetBool(Constants.IsDamaged, true);
        }
        healthBar.UpdateHealthBar();
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
        playerAnimator.SetBool(Constants.IsDamaged, false);   
        GetComponent<PlayerInput>().DeactivateInput();
        playerAnimator.SetBool(Constants.IsDead, true);
        yield return new WaitForSeconds(playerAnimator.GetCurrentAnimatorStateInfo(0).length);
        SceneManager.LoadScene(Constants.EndScene);
    }
    
    private IEnumerator FlashRoutine()
    {
        spriteRenderer.material = flashMaterial;    
        yield return new WaitForSeconds(duration);  
        spriteRenderer.material = originalMaterial; 
        flashRoutine = null;                        
    }

    private void Flash()
    {
        if (flashRoutine != null)  
        {
            StopCoroutine (flashRoutine);   
        }
        flashRoutine = StartCoroutine(FlashRoutine());  
    }
    
    IEnumerator ShootAnimation()
    {
        cannonPivot.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Decorations");
        yield return new WaitForSeconds(duration);
        cannonPivot.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Background");
    }
    
}
