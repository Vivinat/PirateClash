using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class FinalScreenOptions : MonoBehaviour
{
    public void PlayAgain()
    {
        GameController.instance.finalScore = 0;
        SceneManager.LoadScene(Constants.StageScene);
    }

    public void ReturnMainMenu()
    {
        SceneManager.LoadScene(Constants.MainMenuScene);
    }
}
