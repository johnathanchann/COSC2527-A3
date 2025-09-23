

using UnityEngine;

namespace BehavorialTree {

public class JustShoot : IStrategy {
  private readonly PlayerBt _agent;

  public JustShoot(PlayerBt agent) { _agent = agent; }

  public Node.Status Process() {
    Vector2 ballPos = _agent.GetBallWorldPosition();
    Vector2 playerPos = _agent.GetPlayerWorldPosition();
    if (Vector2.Distance(playerPos, ballPos) > _agent.KickDistance)
      return Node.Status.Failure;

    float sign = _agent.Team == Team.Left ? 1f : -1f;
    Vector2 goalCenter = new Vector2(sign * _agent.FieldHalfWidth, 0f);
    Vector2 dir = (goalCenter - ballPos).normalized;

    _agent.DesiredDirection = dir;
    _agent.DesiredKick = KickType.Straight;
    return Node.Status.Success;
  }

  public void Reset() {}
}
}
