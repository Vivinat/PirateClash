using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public int finalScore;
    private ScoreManager scoreManager;
    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void CallNextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    } 
}
