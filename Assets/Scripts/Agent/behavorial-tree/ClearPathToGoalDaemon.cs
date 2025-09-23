

using UnityEngine;

namespace BehavorialTree {

public class ClearPathToGoalDaemon : IStrategy {
  private readonly PlayerBt _agent;

  public ClearPathToGoalDaemon(PlayerBt agent) { _agent = agent; }

  public Node.Status Process() {

    Vector2 ball = _agent.GetBallWorldPosition();

    float sign = _agent.Team == Team.Left ? 1f : -1f;
    Vector2 goalTop =
        new Vector2(sign * _agent.FieldHalfWidth, _agent.GoalHalfHeight);
    Vector2 goalBottom =
        new Vector2(sign * _agent.FieldHalfWidth, -_agent.GoalHalfHeight);

    LayerMask mask = _agent.obstacleMask;
    bool clearToTop = !Physics2D.Linecast(ball, goalTop, mask);
    bool clearToBottom = !Physics2D.Linecast(ball, goalBottom, mask);

    return (clearToTop && clearToBottom) ? Node.Status.Success
                                         : Node.Status.Failure;
  }

  public void Reset() {}
}
}
