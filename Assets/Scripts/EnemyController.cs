using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, ITakeDamage
{
    public static Action<EnemyController> ON_ENEMY_DESTROY;
    [SerializeField] protected EnemyType enemyType;
    [SerializeField] protected GameObject model;
    [SerializeField] protected GameObject destroyVfx;
    [SerializeField] protected GameObject warningObj;
    [SerializeField] protected float moveSpeed;
    public EnemyType EnemyType => enemyType;

    public bool playable;
    protected Transform target;

    public void StartSpawnEnemy(float delay)
    {
        StartCoroutine(CorStartSpawnEnemy(delay));
    }

    private IEnumerator CorStartSpawnEnemy(float delay)
    {
        playable = false;
        warningObj.SetActive(true);
        ToggleModel(false);
        yield return new WaitForSeconds(delay);
        warningObj.SetActive(false);
        ToggleModel(true);
        playable = true;
    }

    public void SetPlayer(Transform player)
    {
        target = player;
    }

    public void ToggleModel(bool isActive)
    {
        model.SetActive(isActive);
    }

    private void Update()
    {
        if (!playable)
            return;
        Move();
    }

    protected virtual void Move()
    {
        if (target == null)
            return;
        Vector2 moveVec = (target.position - transform.position).normalized;
        transform.position = Vector2.Lerp(transform.position, (Vector2)transform.position + moveVec * moveSpeed, Time.deltaTime);
    }

    protected virtual void Rotate()
    {
        // float angle = Mathf.Atan2(inputData.rotateVec.y, inputData.rotateVec.x) * Mathf.Rad2Deg - 90f;
        // model.rotation = Quaternion.Lerp(model.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotateSpeed);
    }

    public void TakeDamage()
    {
        if (!playable)
            return;
        playable = false;
        ON_ENEMY_DESTROY?.Invoke(this);
        ToggleModel(false);
        ShowVfx();
    }

    private void ShowVfx()
    {
        StartCoroutine(CorShowVfx());
    }

    private IEnumerator CorShowVfx()
    {
        destroyVfx.SetActive(true);
        yield return new WaitForSeconds(1f);
        destroyVfx.SetActive(false);
    }
}
