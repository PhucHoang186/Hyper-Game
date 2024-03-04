using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScreenPanelUI : MonoBehaviour
{
    [SerializeField] GameObject container;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text maxScoreText;

    public void Init(int score, int maxScore)
    {
        scoreText.text = score.ToString();
        maxScoreText.text = maxScore.ToString();
    }

    public void TogglePanel(bool isActive)
    {
        container.SetActive(isActive);
    }


}
