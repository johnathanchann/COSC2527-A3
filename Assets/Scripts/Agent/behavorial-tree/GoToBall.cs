

using UnityEngine;

namespace BehavorialTree {

public class GoToBall : IStrategy {
  private readonly PlayerBt _agent;

  public GoToBall(PlayerBt agent) { _agent = agent; }

  public Node.Status Process() {
    Vector2 ballPos = _agent.GetBallWorldPosition();
    Vector2 selfPos = _agent.GetPlayerWorldPosition();
    Vector2 diff = ballPos - selfPos;

    if (diff.sqrMagnitude < 0.001f) {
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
