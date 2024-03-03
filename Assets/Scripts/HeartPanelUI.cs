using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPanelUI : MonoBehaviour
{
    [SerializeField] Transform container;
    [SerializeField] HeartIcon heartIconPrefab;
    [SerializeField] List<HeartIcon> heartIcons;
    private int heartAmount;

    public void SetUpHearts(int heartAmount)
    {
        this.heartAmount = heartAmount;
        for (int i = 0; i < heartAmount; i++)
        {
            heartIcons.Add(Instantiate(heartIconPrefab, container));
        }
    }

    public void UpdateHeart()
    {
        if (heartAmount - 1 < 0)
            return;

        heartIcons[heartAmount - 1].OnToggleOff();
        heartAmount--;
    }
}
