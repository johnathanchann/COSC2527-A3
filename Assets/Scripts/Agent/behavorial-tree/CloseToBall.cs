using UnityEngine;

namespace BehavorialTree {

public class CloseToBall : IStrategy {
  private readonly PlayerBt _agent;

  public CloseToBall(PlayerBt agent) { _agent = agent; }

  public Node.Status Process() {
    Vector2 ballPos = _agent.GetBallWorldPosition();
    Vector2 selfPos = _agent.GetPlayerWorldPosition();
    return Vector2.Distance(ballPos, selfPos) <= _agent.KickDistance
               ? Node.Status.Success
               : Node.Status.Failure;
  }

  public void Reset() {}
}
}
