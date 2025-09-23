

using UnityEngine;

namespace BehavorialTree {

public class MoveMiddleOfGoal : IStrategy {
  private readonly PlayerBt _agent;

  public MoveMiddleOfGoal(PlayerBt agent) { _agent = agent; }

  public Node.Status Process() {

    Vector2 goalTop = _agent.GetOwnGoalTop();
    Vector2 goalBottom = _agent.GetOwnGoalBottom();

    Vector2 goalMiddle =
        new Vector2(goalTop.x, (goalTop.y + goalBottom.y) * 0.5f);

    Vector2 selfPos = _agent.GetPlayerWorldPosition();
    Vector2 diff = goalMiddle - selfPos;
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
