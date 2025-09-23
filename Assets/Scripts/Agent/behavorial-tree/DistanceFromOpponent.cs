

using UnityEngine;
using System.Linq;

namespace BehavorialTree {

public class DistanceFromOpponent : IStrategy {
  private readonly PlayerBt _agent;
  private readonly int _team;
  private readonly float _distance;

  public DistanceFromOpponent(PlayerBt agent, int team, float distance) {
    _agent = agent;
    _team = team;
    _distance = distance;
  }

  public Node.Status Process() {

    var me =
        _agent.GameManager.players.FirstOrDefault(p => p.team == (Team)_team);
    var opp =
        _agent.GameManager.players.FirstOrDefault(p => p.team != (Team)_team);
    if (me == null || opp == null)
      return Node.Status.Failure;

    Vector2 mePos = (Vector2)me.transform.position;
    Vector2 oppPos = (Vector2)opp.transform.position;

    return Vector2.Distance(mePos, oppPos) < _distance ? Node.Status.Success
                                                       : Node.Status.Failure;
  }

  public void Reset() {}
}
}
