using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeartHandle : MonoBehaviour
{
    public static Action<bool> ON_PLAYER_HIT;
    public int maxHeart;
    private int currentHeart;

    void Start()
    {
        currentHeart = maxHeart;
    }

    public bool TakeDamage()
    {
        currentHeart--;
        bool isLose = currentHeart <= 0;
        ON_PLAYER_HIT?.Invoke(isLose);
        return isLose;
    }
}
