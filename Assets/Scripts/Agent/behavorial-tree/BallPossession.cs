

using UnityEngine;

namespace BehavorialTree {

public class BallPossession : IStrategy {
  private readonly PlayerBt _agent;
  private readonly int _team;
  private readonly bool _isMine;
  private const float Threshold = 90f;

  public BallPossession(PlayerBt agent, int team, bool isMine) {
    _agent = agent;
    _team = team;
    _isMine = isMine;
  }

  public Node.Status Process() {
    Vector2 ballPos = _agent.GetBallWorldPosition();
    if (_isMine) {
      if (_agent.Team == (Team)_team) {
        Vector2 myPos = _agent.GetPlayerWorldPosition();
        if (Vector2.Distance(myPos, ballPos) < Threshold)
          return Node.Status.Success;
      }
    } else {
      if (_agent.Team != (Team)_team) {
        Vector2 oppPos = _agent.GetOpponentWorldPosition();
        if (oppPos != Vector2.zero &&
            Vector2.Distance(oppPos, ballPos) < Threshold)
          return Node.Status.Success;
      }
    }
    return Node.Status.Failure;
  }

  public void Reset() {}
}
}
