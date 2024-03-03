using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Gameplay,
    GameOver,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] EnemySpawner enemySpawner;
    [SerializeField] PlayerController player;
    [SerializeField] ScoreManager scoreManger;
    private GameState currentGameState;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        enemySpawner.Init(player);
        UIManager.Instance.SetUpHearts(player.GetMaxHeart());
        PlayerHeartHandle.ON_PLAYER_HIT += OnPlayerHit;
    }

    private void OnDestroy()
    {
        PlayerHeartHandle.ON_PLAYER_HIT -= OnPlayerHit;
        if (Instance == this)
            Instance = null;
    }

    private void OnPlayerHit(bool isLose)
    {
        UIManager.Instance.UpdateHeart();
    }

    private void OnChangeState(GameState newState)
    {
        if (currentGameState == newState)
            return;
        currentGameState = newState;

        switch (newState)
        {
            case GameState.Gameplay:
                break;
            case GameState.GameOver:

                break;
        }
    }

    private void Endgame()
    {
        // UIManager.Instance.
    }

}