using UnityEngine;

namespace BehavorialTree {

public class BallMoving : IStrategy {
  private readonly PlayerBt _agent;
  private readonly float _threshold;

  public BallMoving(PlayerBt agent, float threshold = 0.2f) {
    _agent = agent;
    _threshold = threshold;
  }

  public Node.Status Process() {
    return _agent.GetBallWorldVelocity().magnitude > _threshold
               ? Node.Status.Success
               : Node.Status.Failure;
  }

  public void Reset() {}
}
}
