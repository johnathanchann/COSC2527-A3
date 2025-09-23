using UnityEngine;

namespace BehavorialTree {

public class InterceptBall : IStrategy {
  private readonly PlayerBt _agent;
  private readonly float _predictionTime;

  public InterceptBall(PlayerBt agent, float predictionTime = 0.5f) {
    _agent = agent;
    _predictionTime = predictionTime;
  }

  public Node.Status Process() {
    Vector2 ballPos = _agent.GetBallWorldPosition();
    Vector2 ballVel = _agent.GetBallWorldVelocity();
    Vector2 target = ballPos + ballVel * _predictionTime;
    Vector2 selfPos = _agent.GetPlayerWorldPosition();
    Vector2 diff = target - selfPos;

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
