using UnityEngine;
public class MirrorAgent : AgentBase
{
    public override (Vector2 dir, KickType kick) GetAction()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 ballPosition = player.ball.transform.position;
        Vector2 mirroredPosition = new Vector2(ballPosition.x - (playerPosition.x - ballPosition.x), ballPosition.y);
        Vector2 direction = (mirroredPosition - playerPosition).normalized;
        return (direction, KickType.Straight);
    }
}