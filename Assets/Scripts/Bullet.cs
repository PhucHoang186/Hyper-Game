using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, ITakeDamage
{
    [SerializeField] float speed;
    [SerializeField] GameObject model;
    [SerializeField] GameObject vfx;

    public void TakeDamage()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        transform.position = Vector2.Lerp(transform.position, transform.position + transform.right * speed, Time.deltaTime);
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (collider.TryGetComponent<PlayerController>(out var Player))
            {
                Player.TakeDamage();
                DestroyVfx();
            }
        }
    }

    private void DestroyVfx()
    {
        vfx.SetActive(true);
        model.SetActive(false);
        Destroy(gameObject, 2f);
    }
}
