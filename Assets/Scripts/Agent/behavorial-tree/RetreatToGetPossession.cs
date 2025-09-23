

using UnityEngine;

namespace BehavorialTree {

public class RetreatToGetPossession : IStrategy {
  private readonly PlayerBt _agent;
  private const float Delta = 42f;

  public RetreatToGetPossession(PlayerBt agent) { _agent = agent; }

  public Node.Status Process() {

    float sign = _agent.Team == Team.Left ? 1f : -1f;
    float selfGoalX = sign * -_agent.FieldHalfWidth;

    float halfField = Mathf.Abs(selfGoalX);
    float fieldDelta = halfField * 0.25f;

    Vector2 playerPos = _agent.GetPlayerWorldPosition();
    Vector2 ballPos = _agent.GetBallWorldPosition();

    float distPlayerGoal = Mathf.Abs(selfGoalX - playerPos.x);
    if (distPlayerGoal > halfField + fieldDelta)
      return Node.Status.Failure;

    float distBallGoal = Mathf.Abs(selfGoalX - ballPos.x);
    if (distBallGoal + Delta < distPlayerGoal)
      return Node.Status.Success;

    return Node.Status.Failure;
  }

  public void Reset() {}
}
}
