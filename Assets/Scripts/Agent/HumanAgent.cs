using UnityEngine;
public class HumanAgent : AgentBase
{
    Vector2 lastDirection = Vector2.zero;
    KickType lastKick = KickType.None;
    public void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        KickType nextKick = KickType.None;
        if (Input.GetKey(KeyCode.Space))
            nextKick = KickType.Straight;
        else if (Input.GetKey(KeyCode.Q))
            nextKick = KickType.Left;
        else if (Input.GetKey(KeyCode.E))
            nextKick = KickType.Right;

        lastKick = nextKick;
        lastDirection = new Vector2(x, y).normalized;
        // sprint removed
    }

    public override (Vector2 dir, KickType kick) GetAction()
    {
        return (lastDirection, lastKick);
    }
}