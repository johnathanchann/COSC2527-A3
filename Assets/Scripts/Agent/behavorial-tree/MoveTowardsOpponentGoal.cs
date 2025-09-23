

using UnityEngine;

namespace BehavorialTree {

public class MoveTowardsOpponentGoal : IStrategy {
  private readonly PlayerBt _agent;

  public MoveTowardsOpponentGoal(PlayerBt agent) { _agent = agent; }

  public Node.Status Process() {

    float sign = _agent.Team == Team.Left ? 1f : -1f;
    Vector2 goalCenter = new Vector2(sign * _agent.FieldHalfWidth, 0f);

    Vector2 selfPos = _agent.GetPlayerWorldPosition();
    Vector2 toGoal = goalCenter - selfPos;
    if (toGoal.sqrMagnitude < 0.01f) {

      _agent.DesiredDirection = Vector2.zero;
      _agent.DesiredKick = KickType.None;
      return Node.Status.Failure;
    }

    _agent.DesiredDirection = toGoal.normalized;
    _agent.DesiredKick = KickType.None;
    return Node.Status.Running;
  }

  public void Reset() {}
}
}
