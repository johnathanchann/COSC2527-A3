using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MagnusEffect : MonoBehaviour
{
    public float radius = 0.5f;

    public float airDensity = 0.1f;
    private Rigidbody2D rb;
    private float S => 4f / 3f * Mathf.PI * airDensity * Mathf.Pow(radius, 3f);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 v = rb.linearVelocity;
        if (v.sqrMagnitude < 0.01f) return;
        float omega = rb.angularVelocity * Mathf.Deg2Rad;
        Vector2 perp = new Vector2(-v.y, v.x);
        Vector2 Fm = S * omega * perp;
        rb.AddForce(Fm);
    }
}
