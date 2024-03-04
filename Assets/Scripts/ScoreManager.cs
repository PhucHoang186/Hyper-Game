using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    public const string MaxScoreString = "Max Score";
    private int maxScore;
    private int currentScore;
    public int CurrentScore => currentScore;
    public int MaxScore => maxScore;

    void Start()
    {
        maxScore = PlayerPrefs.GetInt(MaxScoreString, 0);
        EnemyController.ON_ENEMY_DESTROY += OnEnemyDestroy;
    }

    private void OnDestroy()
    {
        EnemyController.ON_ENEMY_DESTROY -= OnEnemyDestroy;
    }

    private void OnEnemyDestroy(EnemyController enemy)
    {
        UpdateScore();
        UIManager.Instance.UpdateScore(currentScore);
    }

    public void UpdateScore()
    {
        currentScore++;
        if (currentScore > maxScore)
        {
            maxScore = currentScore;
            PlayerPrefs.SetInt(MaxScoreString, maxScore);
        }
    }
}
