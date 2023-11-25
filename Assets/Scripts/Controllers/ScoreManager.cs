using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    public int score;

    private void Start()
    {
        UpdateScoreText();
    }

    public void AddScore()
    {
        score += 1;
        GameController.instance.finalScore += 1;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }
}

