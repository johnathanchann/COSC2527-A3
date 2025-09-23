

using UnityEngine;

namespace BehavorialTree {

public class Chase : IStrategy {
  private readonly PlayerBt _agent;

  public Chase(PlayerBt agent) { _agent = agent; }

  public Node.Status Process() {
    Vector2 oppPos = _agent.GetOpponentWorldPosition();
    if (oppPos == Vector2.zero)
      return Node.Status.Failure;

    Vector2 selfPos = _agent.GetPlayerWorldPosition();
    Vector2 toOpp = oppPos - selfPos;
    if (toOpp.sqrMagnitude < 0.001f) {
      _agent.DesiredDirection = Vector2.zero;
      _agent.DesiredKick = KickType.None;
      return Node.Status.Success;
    }

    _agent.DesiredDirection = toOpp.normalized;
    _agent.DesiredKick = KickType.None;
    return Node.Status.Running;
  }

  public void Reset() {}
}
}