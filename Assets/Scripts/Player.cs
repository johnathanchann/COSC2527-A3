using UnityEngine;

public enum Team
{
    Left = 0,
    Right = 1
}
public enum KickType
{
    None = 0,
    Straight = 1,
    Left = 2,
    Right = 3,
}

public class Player : MonoBehaviour
{
    public GameObject ball;
    public float KickRadius = 0.4f;
    public float KickPower = 5f;
    public float MaxMultiplier = 1.5f; // unused but kept for prefab compatibility
    public float NormalSpeed = 1.5f;
    public Team team = Team.Left;
    public float AccelerationTime = 0.1f;
    public float MaxAcceleration = 10f;
    private Rigidbody2D _rb;

    public Transform SpawnPoint;
    [SerializeField]
    private SoundFxManager SoundFxManager;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }


    public void ApplyAction(Vector2 dir, KickType kickType = KickType.None)
    {
        Shoot(kickType);
        float curMaxSpeed = NormalSpeed;
        float curAccelTime = AccelerationTime;
        Vector2 force = GetDesiredForce(dir, curAccelTime, curMaxSpeed);
        _rb.AddForce(force);
    }
    private Vector2 GetDesiredForce(Vector2 direction, float accelerationTime, float maxSpeed)
    {
        Vector2 desiredVel = direction.normalized * maxSpeed;
        Vector2 steering = desiredVel - _rb.linearVelocity;
        steering = Vector2.ClampMagnitude(steering, MaxAcceleration);
        return _rb.mass * steering;
    }
    public void ApplyAction(Vector2 dir, bool kick)
    {
        if (kick)
        {
            ApplyAction(dir, KickType.Straight);
        }
        else
        {
            ApplyAction(dir, KickType.None);
        }
    }


    public void Shoot(KickType kickType)
    {
        if (kickType == KickType.None) return;
        if (CanShoot())
        {
            Vector2 direction = ((Vector2)ball.transform.position - (Vector2)transform.position).normalized;
            float sign = 0;
            if (kickType == KickType.Left)
            {
                sign = -1;
            }
            else if (kickType == KickType.Right)
            {
                sign = 1;
            }

            float multiplier = 1;
            direction = Quaternion.Euler(0, 0, sign * -45f) * direction;
            ball.GetComponent<Rigidbody2D>().angularVelocity = sign * 2000f;
            ball.GetComponent<Rigidbody2D>().linearVelocity = multiplier * direction * KickPower;
            if (SoundFxManager != null)
            {
                SoundFxManager.PlayBallKicked(ball.transform.position);
            }
        }
    }
    public void SetVelocity(Vector2 velocity)
    {
        _rb.linearVelocity = velocity;
    }

    public bool CanShoot()
    {
        float distance = Vector2.Distance(transform.position, ball.transform.position);
        return distance <= KickRadius;
    }

    public void Refresh()
    {
        transform.position = SpawnPoint.position;
        _rb.linearVelocity = new Vector2(0, 0);
    }
}
