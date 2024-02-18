using System.Collections;
using System.Collections.Generic;
using MilkShake;
using NaughtyAttributes;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    [SerializeField] Shaker shaker;
    [SerializeField] ShakePreset shakePreset;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    [Button]
    public void ShakeCamera()
    {
        shaker?.Shake(shakePreset);
    }
}
