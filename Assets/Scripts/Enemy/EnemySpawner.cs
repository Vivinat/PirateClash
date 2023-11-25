using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]private GameObject swarmerPrefab;
    
    void Start()
    {
        float spawnTime = PlayerPrefs.GetFloat(Constants.SpawnTimePref);
        StartCoroutine(spawnEnemy(spawnTime, swarmerPrefab));
    }

    private IEnumerator spawnEnemy(float swarmerInterval, GameObject enemy)
    {
        yield return new WaitForSecondsRealtime(swarmerInterval);  
        GameObject newEnemy = Instantiate(enemy, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f), Quaternion.identity);
        StartCoroutine(spawnEnemy(swarmerInterval, enemy));    
    }
}
