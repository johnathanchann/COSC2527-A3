using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Transform SpawnPoint;
    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    public void SetVelocity(Vector2 velocity)
    {
        _rb.linearVelocity = velocity;
    }

    public void Refresh()
    {
        transform.position = SpawnPoint.position;
        _rb.linearVelocity = new Vector2(0, 0);
    }

}
