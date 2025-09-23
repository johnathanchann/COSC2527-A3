

using UnityEngine;

namespace BehavorialTree {
public class KickBallAway : IStrategy {
  private readonly PlayerBt _agent;
  private readonly float _kickDistance;

  public KickBallAway(PlayerBt agent, float kickDistance) {
    _agent = agent;
    _kickDistance = kickDistance;
  }

  public Node.Status Process() {
    Vector2 ballPos = _agent.GetBallWorldPosition();
    Vector2 playerPos = _agent.GetPlayerWorldPosition();

    if (Vector2.Distance(playerPos, ballPos) > _kickDistance)
      return Node.Status.Failure;

    Vector2 oppPos = _agent.GetOpponentWorldPosition();
    Vector2 dir;
    if (oppPos != Vector2.zero) {

      dir = (ballPos - oppPos).normalized;
    } else {

      dir = Random.insideUnitCircle.normalized;
    }

    _agent.DesiredDirection = dir;
    _agent.DesiredKick = KickType.Straight;
    return Node.Status.Success;
  }

  public void Reset() {}
}
}
