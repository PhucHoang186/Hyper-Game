using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] EnemySpawner enemySpawner;
    [SerializeField] PlayerController player;
    [SerializeField] ScoreManager scoreManger;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        EnemyController.ON_ENEMY_DESTROY += OnEnemyDestroy;
        enemySpawner.Init(player);
    }

    private void OnEnemyDestroy(EnemyController enemy)
    {
        UpdateScore();
        CheckSpawnAndDestroyEnemy(enemy);
    }

    private void CheckSpawnAndDestroyEnemy(EnemyController enemy)
    {
        enemySpawner.DestroyEnemy(enemy, 2f);
        enemySpawner.CheckSpawnEnemy();
    }

    private void UpdateScore()
    {
        scoreManger.UpdateScore();
        UIManager.Instance.UpdateScore(scoreManger.CurrentScore);
    }

    private void OnDestroy()
    {
        EnemyController.ON_ENEMY_DESTROY -= OnEnemyDestroy;
        if (Instance == this)
            Instance = null;
    }
}
