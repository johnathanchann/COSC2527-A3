using UnityEngine;

namespace BehavorialTree {

public class BallInCorner : IStrategy {
  private readonly PlayerBt _agent;
  private readonly float _threshold;

  public BallInCorner(PlayerBt agent, float threshold = 2f) {
    _agent = agent;
    _threshold = threshold;
  }

  public Node.Status Process() {
    Vector2 pos = _agent.GetBallWorldPosition();
    float fw = _agent.FieldHalfWidth;
    float gh = _agent.GoalHalfHeight;

    bool nearSide = Mathf.Abs(Mathf.Abs(pos.x) - fw) < _threshold;
    bool pastGoal = Mathf.Abs(pos.y) > gh;

    return (nearSide && pastGoal) ? Node.Status.Success : Node.Status.Failure;
  }

  public void Reset() {}
}
}
