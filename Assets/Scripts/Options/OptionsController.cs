using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

public class OptionsController : MonoBehaviour
{
    [SerializeField] private Options options;
    [SerializeField] private Slider spawnSlider;
    [SerializeField] private TextMeshProUGUI spawnText;
    [SerializeField] private Slider sessionSlider;
    [SerializeField] private TextMeshProUGUI sessionText;
    
    private void Awake()
    {
        spawnSlider.value = PlayerPrefs.GetFloat(Constants.SpawnTimePref);
        sessionSlider.value = PlayerPrefs.GetFloat(Constants.SessionTimePref);
    }
    
    private void Start()
    {
        spawnSlider.onValueChanged.AddListener((v) =>
        {
            spawnText.text = v.ToString("0.0");
            options.enemySpawnTime = v;
        });
        sessionSlider.onValueChanged.AddListener((f) =>
        {
            sessionText.text = f.ToString("0.0");
            options.gameSessionTime = f;
        });
    }

    public void ReturnToMenu()
    {
        PlayerPrefs.SetFloat(Constants.SessionTimePref, options.gameSessionTime);
        PlayerPrefs.SetFloat(Constants.SpawnTimePref, options.enemySpawnTime);
        PlayerPrefs.Save();
        Debug.Log("Saving Spawner Timer to Prefs " + PlayerPrefs.GetFloat(Constants.SpawnTimePref));
        Debug.Log("Saving Session Timer to Prefs " + PlayerPrefs.GetFloat(Constants.SessionTimePref));
        SceneManager.LoadScene("Menu");
    }
}
