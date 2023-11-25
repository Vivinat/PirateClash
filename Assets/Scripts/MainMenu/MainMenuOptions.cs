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
