using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

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

    [SerializeField] ScorePanelUI scorePanel;

    public void UpdateScore(int score)
    {
        scorePanel.UpdateScore(score);
    }
}
