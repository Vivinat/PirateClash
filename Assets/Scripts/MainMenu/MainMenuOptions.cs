using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class MainMenuOptions : MonoBehaviour
{
    public void TutorialScene()
    {
        if (!PlayerPrefs.HasKey(Constants.SpawnTimePref))
        {
            PlayerPrefs.SetFloat(Constants.SessionTimePref, 1f);
            PlayerPrefs.SetFloat(Constants.SpawnTimePref, 3f);    
            PlayerPrefs.Save();
        }
        SceneManager.LoadScene(Constants.TutorialScene);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(Constants.StageScene);
    }

    public void OptionsScene()
    {
        SceneManager.LoadScene(Constants.OptionsMenuScene);
    }
    
}
