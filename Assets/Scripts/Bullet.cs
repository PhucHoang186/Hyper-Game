using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, ITakeDamage
{
    [SerializeField] float speed;

    public void TakeDamage()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        transform.position = Vector2.Lerp(transform.position, transform.position + transform.right * speed, Time.deltaTime);
    }
}
