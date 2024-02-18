using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    public const string MaxScore = "Max Score";
    private int maxScore;
    private int currentScore;
    public int CurrentScore => currentScore;

    void Start()
    {
        maxScore = PlayerPrefs.GetInt(MaxScore, 0);
    }

    public void UpdateScore()
    {
        currentScore ++;
        if(currentScore > maxScore)
        {
            maxScore = currentScore;
            PlayerPrefs.SetInt(MaxScore, maxScore);
        }
    }
}
