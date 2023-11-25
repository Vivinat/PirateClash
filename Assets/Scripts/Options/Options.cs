using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Options")]
public class Options : ScriptableObject
{
    [Range(1.0f, 3.0f)] public float gameSessionTime = 1.0f;
    [Range(3f, 10f)] public float enemySpawnTime = 3.0f;
}
