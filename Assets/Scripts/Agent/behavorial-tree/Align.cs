

using UnityEngine;

namespace BehavorialTree {

public class Align : IStrategy {
  private readonly PlayerBt _agent;
  public Align(PlayerBt agent) { _agent = agent; }

  public Node.Status Process() {

    Vector2 ballPos = _agent.GetBallWorldPosition();
    Vector2 playerPos = _agent.GetPlayerWorldPosition();

    float sign = _agent.Team == Team.Left ? 1f : -1f;
    Vector2 goalCenter = new Vector2(sign * _agent.FieldHalfWidth, 0f);

    Vector2 shotDir = (goalCenter - ballPos).normalized;

    float offset = _agent.BallRadius * 1.5f;
    Vector2 alignPos = ballPos - shotDir * offset;

    Vector2 diff = alignPos - playerPos;
    if (diff.sqrMagnitude < 0.35f) {
      _agent.DesiredDirection = Vector2.zero;
      _agent.DesiredKick = KickType.None;
      return Node.Status.Success;
    }

    _agent.DesiredDirection = diff.normalized;
    _agent.DesiredKick = KickType.None;
    return Node.Status.Running;
  }

  public void Reset() {}
}
}
