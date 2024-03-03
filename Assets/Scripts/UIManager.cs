using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] ScorePanelUI scorePanel;
    [SerializeField] HeartPanelUI heartPanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void UpdateHeart()
    {
        heartPanel.UpdateHeart();
    }

    public void UpdateScore(int score)
    {
        scorePanel.UpdateScore(score);
    }

    public void SetUpHearts(int heartAmount)
    {
        heartPanel.SetUpHearts(heartAmount);
    }
}
