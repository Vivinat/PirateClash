using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BulletType
{
    player,
    enemy,
} 

public class BulletHit : MonoBehaviour
{
    [SerializeField] private int lifeTime;
    [SerializeField] private BulletType bulletType;
    [SerializeField] private int bulletDamage;
    private void OnTriggerEnter2D(Collider2D col)
    {
        switch (bulletType)
        {
            case BulletType.enemy:
                if (col.CompareTag("Player"))
                {
                    PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
                    player.TakeDamage(bulletDamage);
                    Destroy(gameObject);    
                }
                break;
            case BulletType.player:
                if (col.CompareTag("Enemy"))
                {
                    col.gameObject.GetComponent<EnemyController>().TakeDamage();
                    Destroy(gameObject);    
                }
                break;
        }
        if (col.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(BulletDuration());
    }

    IEnumerator BulletDuration()
    {
        yield return new WaitForSecondsRealtime(lifeTime);
        Destroy(gameObject);
    }
}