

using UnityEngine;

namespace BehavorialTree {

public class MoveBetweenOpponentAndGoal : IStrategy {
  private readonly PlayerBt _agent;
  private readonly float _ratio;

  public MoveBetweenOpponentAndGoal(PlayerBt agent, float ratio = 0.5f) {
    _agent = agent;
    _ratio = Mathf.Clamp01(ratio);
  }

  public Node.Status Process() {
    Vector2 oppPos = _agent.GetOpponentWorldPosition();
    if (oppPos == Vector2.zero)
      return Node.Status.Failure;

    float sign = _agent.Team == Team.Left ? 1f : -1f;
    Vector2 goalCenter = new Vector2(sign * _agent.FieldHalfWidth, 0f);

    Vector2 target = Vector2.Lerp(oppPos, goalCenter, _ratio);

    Vector2 self = _agent.GetPlayerWorldPosition();
    Vector2 dir = target - self;
    if (dir.sqrMagnitude < 0.01f) {
      _agent.DesiredDirection = Vector2.zero;
      _agent.DesiredKick = KickType.None;
      return Node.Status.Success;
    }

    _agent.DesiredDirection = dir.normalized;
    _agent.DesiredKick = KickType.None;
    return Node.Status.Running;
  }

  public void Reset() {}
}
}
