using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    public const string MaxScore = "Max Score";
    private int maxScore;
    private int currentScore;

    void Start()
    {
        maxScore = PlayerPrefs.GetInt(MaxScore, 0);
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
            PlayerPrefs.SetInt(MaxScore, maxScore);
        }
    }
}
