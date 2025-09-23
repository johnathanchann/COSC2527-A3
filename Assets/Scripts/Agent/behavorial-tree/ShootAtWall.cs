

using UnityEngine;

namespace BehavorialTree {

public class ShootAtWall : IStrategy {
  private readonly PlayerBt _agent;

  public ShootAtWall(PlayerBt agent) { _agent = agent; }

  public Node.Status Process() {

    Vector2 ballPos = _agent.GetBallWorldPosition();
    Vector2 playerPos = _agent.GetPlayerWorldPosition();
    if (Vector2.Distance(playerPos, ballPos) > _agent.KickDistance)
      return Node.Status.Failure;

    float sign = _agent.Team == Team.Left ? 1f : -1f;
    Vector2 goalDir = new Vector2(sign, 0f);

    Vector2 perp = new Vector2(-goalDir.y, goalDir.x);

    _agent.DesiredDirection = perp;
    _agent.DesiredKick = KickType.Straight;
    return Node.Status.Success;
  }

  public void Reset() {}
}
}
