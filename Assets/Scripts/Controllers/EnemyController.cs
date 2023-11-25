using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

public enum EnemyState
{
    Follow,
    Die,
    Attack,
};

public enum EnemyType
{
    Chaser,
    Shooter,
};

public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyState currentState = EnemyState.Follow; 
        [SerializeField] private EnemyType enemyType;

        public int life;
        public int maxLife;
        
        [SerializeField] private float speed;         
        [SerializeField] private float attackRange;
        [SerializeField] private int cooldown;
        [SerializeField] private int attackDamage;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float shootSpeed = 2;
        
        [SerializeField] private Material flashMaterial;
        [SerializeField] private float duration;

        [SerializeField] private HealthBar healthBar;
        
        [SerializeField] private ScoreManager scoreManager;
        
        private SpriteRenderer spriteRenderer;
        private Material originalMaterial;
        private Coroutine flashRoutine;
        
        private bool cooldownAttack = false;

        private Animator enemyAnimator;
        private PlayerController player;
        private Transform playerTransform;

       
        
        void Start()
        {
            maxLife = life;
            player = FindObjectOfType<PlayerController>();
            playerTransform = player.GetComponent<Transform>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            enemyAnimator = GetComponent<Animator>();
            scoreManager = FindObjectOfType<ScoreManager>();
            originalMaterial = spriteRenderer.material;
        }
        
        void Update()
        {  
            switch(currentState)
            {
                case(EnemyState.Follow):
                Follow();
                break;
                case(EnemyState.Die):
                Die();
                break;
                case(EnemyState.Attack):
                Attack();
                break;
            }

            if(Vector3.Distance(transform.position, player.transform.position) <= attackRange)  
            {
                currentState = EnemyState.Attack;   
            }

            if (Vector3.Distance(transform.position, player.transform.position) >= attackRange)
            {
                currentState = EnemyState.Follow;    
            }
        }
        
        void Follow()
        {
            Vector3 direction = playerTransform.position - transform.position; 
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90; 
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle)); 
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 200f * Time.deltaTime);
            
            if (Vector3.Distance(transform.rotation.eulerAngles, rotation.eulerAngles) < 10.0f)
            {
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
            }
        }


        void Attack()
        {
            if (!cooldownAttack)
            {   
                switch(enemyType)
                {
                    case(EnemyType.Chaser):
                        break;
                    case(EnemyType.Shooter):
                        Vector2 direction = (playerTransform.position - transform.position).normalized;
                        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                        Rigidbody2D enemyBulletRb = bullet.GetComponent<Rigidbody2D>();
                        enemyBulletRb.velocity = direction * shootSpeed;
                        StartCoroutine(CoolDown());
                        break;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (enemyType == EnemyType.Chaser)
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    ChaserAutoDestruction();
                }
            }
        }

        private IEnumerator CoolDown(){
            cooldownAttack = true;      
            yield return new WaitForSecondsRealtime(cooldown);
            cooldownAttack = false; 
        }
        
        public void TakeDamage()
        {
            Flash();
            life--;
            
            if (life <= maxLife/2)
            {
                enemyAnimator.SetBool(Constants.IsDamaged, true);
            }
            healthBar.UpdateHealthBar();
            if (life <= 0)
            {
                scoreManager.AddScore();
                currentState = EnemyState.Die;
            }
        }

        private void ChaserAutoDestruction()
        {
            player.TakeDamage(attackDamage);
            Die();
        }

        private void Die()
        {
            StartCoroutine(WaitForAnimationToEnd());
        }
        
        IEnumerator WaitForAnimationToEnd()
        {
            cooldownAttack = true;   
            enemyAnimator.SetBool(Constants.IsDamaged, false);   
            enemyAnimator.SetBool(Constants.IsDead, true);
            yield return new WaitForSeconds(enemyAnimator.GetCurrentAnimatorStateInfo(0).length);
            Destroy(gameObject);
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
}