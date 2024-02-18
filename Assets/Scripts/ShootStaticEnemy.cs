using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootStaticEnemy : EnemyController
{
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] Transform leftShootPoint;
    [SerializeField] Transform rightShootPoint;
    [SerializeField] Transform turret;
    [SerializeField] float rotateSpeed;
    [SerializeField] float shootDelay;
    private float currentDelayTime;
    private float angle;

    protected override void Move()
    {
        // can't move
    }

    protected override void Rotate()
    {
        angle = turret.eulerAngles.z + Time.deltaTime * rotateSpeed;
        turret.eulerAngles = new Vector3(0f, 0f, angle);
    }

    private void Update()
    {
        if (!playable)
            return;
        Rotate();
        if (currentDelayTime <= 0)
        {
            currentDelayTime = shootDelay;
            SpawnBullet(leftShootPoint);
            SpawnBullet(rightShootPoint);
        }
        else
        {
            currentDelayTime -= Time.deltaTime;
        }
    }

    private void SpawnBullet(Transform shootPoint)
    {
        var bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation, transform);
        Destroy(bullet, 3f);
    }
}
