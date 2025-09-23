using UnityEngine;

namespace BehavorialTree {

public class PredictCounterKick : IStrategy {
  private readonly PlayerBt _agent;
  private readonly float _aheadDistance;

  public PredictCounterKick(PlayerBt agent, float aheadDistance = 1.5f) {
    _agent = agent;
    _aheadDistance = aheadDistance;
  }

  public Node.Status Process() {
    Vector2 ballPos = _agent.GetBallWorldPosition();
    Vector2 selfPos = _agent.GetPlayerWorldPosition();

    float signToOwnGoal = _agent.Team == Team.Left ? -1f : 1f;
    Vector2 goalCenter = new Vector2(signToOwnGoal * _agent.FieldHalfWidth, 0f);
    Vector2 kickDir = (goalCenter - ballPos).normalized;

    Vector2 interceptPoint = ballPos + kickDir * _aheadDistance;
    Vector2 diff = interceptPoint - selfPos;

    if (diff.sqrMagnitude < 0.45f) {
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
