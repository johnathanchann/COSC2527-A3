

using UnityEngine;

namespace BehavorialTree {

public class MoveToOpponentSideAction : IStrategy {
  private readonly PlayerBt _agent;
  private readonly float _targetX;

  public MoveToOpponentSideAction(PlayerBt agent) {
    _agent = agent;

    float signOpp = _agent.Team == Team.Left ? 1f : -1f;
    _targetX = signOpp * (_agent.FieldHalfWidth * 0.5f);
  }

  public Node.Status Process() {
    Vector2 self = _agent.GetPlayerWorldPosition();
    Vector2 target = new Vector2(_targetX, 0f);
    Vector2 diff = target - self;
    if (diff.sqrMagnitude < 0.01f) {
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
