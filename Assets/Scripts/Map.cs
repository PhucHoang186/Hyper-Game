using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] bool showGizMos;
    [SerializeField] PlayerController player;
    public Transform minPoint;
    public Transform maxPoint;

    void Start()
    {
        player.SetBorders(minPoint, maxPoint);
    }

    private void OnDrawGizmos()
    {
        if(!showGizMos)
            return;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(minPoint.position, new Vector2(minPoint.position.x, maxPoint.position.y));
        Gizmos.DrawLine(minPoint.position, new Vector2(maxPoint.position.x, minPoint.position.y));
        Gizmos.DrawLine(new Vector2(maxPoint.position.x, minPoint.position.y), maxPoint.position);
        Gizmos.DrawLine(new Vector2(minPoint.position.x, maxPoint.position.y), maxPoint.position);
    }
}
