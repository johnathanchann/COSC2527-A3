

using UnityEngine;
using System.Linq;

namespace BehavorialTree {

public class ClosestTeamToBall : IStrategy {
  private readonly PlayerBt _agent;
  private readonly int _team;

  public ClosestTeamToBall(PlayerBt agent, int team) {
    _agent = agent;
    _team = team;
  }

  public Node.Status Process() {
    Vector2 ballPos = _agent.GetBallWorldPosition();

    var meObj =
        _agent.GameManager.players.FirstOrDefault(p => p.team == (Team)_team);
    var opObj =
        _agent.GameManager.players.FirstOrDefault(p => p.team != (Team)_team);
    if (meObj == null || opObj == null)
      return Node.Status.Failure;

    var mePos = (Vector2)meObj.transform.position;
    var opPos = (Vector2)opObj.transform.position;

    float dMe = Vector2.Distance(mePos, ballPos);
    float dOp = Vector2.Distance(opPos, ballPos);

    return dMe < dOp ? Node.Status.Success : Node.Status.Failure;
  }

  public void Reset() {}
}
}
