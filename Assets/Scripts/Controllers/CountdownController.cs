using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;

public class CountdownController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        float sessionTime = PlayerPrefs.GetFloat(Constants.SessionTimePref);
        StartCoroutine(SessionTimer(sessionTime));
    }

    private IEnumerator SessionTimer(float sessionTime)
    {
        sessionTime = sessionTime * 60;
        float timer = sessionTime;

        while (timer > 0f)
        {
            text.text = timer.ToString("0");
            yield return new WaitForSeconds(1f); 
            timer -= 1f; 
        }
        GameController.instance.CallNextScene(Constants.EndScene);
    }
}

