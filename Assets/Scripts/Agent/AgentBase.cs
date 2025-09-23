using UnityEngine;
public abstract class AgentBase : MonoBehaviour
{
    protected Player player;

    protected virtual void Start()
    {
        player = GetComponent<Player>();
    }

    public virtual void FixedUpdate()
    {
        var (dir, kick) = GetAction();
        player.ApplyAction(dir, kick);
    }

    public abstract (Vector2 dir, KickType kick) GetAction();
}