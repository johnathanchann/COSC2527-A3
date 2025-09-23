

using UnityEngine;

namespace BehavorialTree {

public class FaceOpponentGoal : IStrategy {
  private readonly PlayerBt _agent;

  public FaceOpponentGoal(PlayerBt agent) { _agent = agent; }

  public Node.Status Process() {

    float sign = _agent.Team == Team.Left ? 1f : -1f;
    Vector2 goalCenter = new Vector2(sign * _agent.FieldHalfWidth, 0f);

    Vector2 playerPos = _agent.GetPlayerWorldPosition();
    Vector2 dir = (goalCenter - playerPos).normalized;

    _agent.DesiredDirection = dir;
    _agent.DesiredKick = KickType.None;
    return Node.Status.Running;
  }

  public void Reset() {}
}
}
