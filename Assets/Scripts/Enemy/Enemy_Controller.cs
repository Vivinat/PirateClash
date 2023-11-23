using System.Collections;
using UnityEngine;

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

public class Enemy_Controller : MonoBehaviour
    {
        [SerializeField] private EnemyState currentState = EnemyState.Follow; 
        [SerializeField] private EnemyType enemyType;

        [SerializeField] private float speed;         
        [SerializeField] private float attackRange;
        [SerializeField] private int cooldown;
        [SerializeField] private int life;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float shootSpeed = 2;
        [SerializeField] private Sprite enemyDamaged;
        
        [SerializeField] private Material flashMaterial;
        [SerializeField] private float duration;
        private SpriteRenderer spriteRenderer;
        private Material originalMaterial;
        private Coroutine flashRoutine;

        public int maxLife;
        private bool cooldownAttack = false;

        private Animator enemyAnimator;
        private PlayerController player;
        
        void Start()
        {
            maxLife = life;
            player = FindObjectOfType<PlayerController>();    
            spriteRenderer = GetComponent<SpriteRenderer>();
            enemyAnimator = GetComponent<Animator>();
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
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }

        void Attack()
        {
            if (!cooldownAttack)
            {   
                switch(enemyType)
                {
                    case(EnemyType.Chaser):
                        ChaserAutoDestruction();
                        break;
                    case(EnemyType.Shooter):
                        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                        Rigidbody2D enemyBulletRb = bullet.GetComponent<Rigidbody2D>();
                        enemyBulletRb.velocity = player.GetComponent<Transform>().position * shootSpeed;
                        StartCoroutine(CoolDown());
                        break;
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

            if (life < maxLife/2)
            {
                spriteRenderer.sprite = enemyDamaged;
            }
            
            if (life <= 0)
            {
                currentState = EnemyState.Die;
            }
        }

        private void ChaserAutoDestruction()
        {
            player.TakeDamage();
            Die();
        }

        private void Die()
        {
            StartCoroutine(WaitForAnimationToEnd());
        }
        
        IEnumerator WaitForAnimationToEnd()
        {
            cooldownAttack = true;   
            enemyAnimator.SetBool("IsDead", true);
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

        public void Flash()
        {
            if (flashRoutine != null)  
            {
                StopCoroutine (flashRoutine);   
            }
            flashRoutine = StartCoroutine(FlashRoutine());  
        }
}