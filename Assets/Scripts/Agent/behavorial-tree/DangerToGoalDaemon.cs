

using UnityEngine;
using System.Collections.Generic;

namespace BehavorialTree {

public class DangerToGoalDaemon : IStrategy {
  private readonly PlayerBt _agent;
  private readonly float _clearDistance;

  public DangerToGoalDaemon(PlayerBt agent, float clearDistance) {
    _agent = agent;
    _clearDistance = clearDistance;
  }

  public Node.Status Process() {

    Vector2 ball = _agent.GetBallWorldPosition();

    List<Player> players = _agent.GameManager.players;
    Player closestOpp = null;
    float minDist = float.MaxValue;

    foreach (var p in players) {
      if (p.team != _agent.Team) {
        Vector2 pos = (Vector2)p.transform.position;
        float d = Vector2.Distance(pos, ball);
        if (d < minDist) {
          minDist = d;
          closestOpp = p;
        }
      }
    }
    if (closestOpp == null || minDist > _clearDistance)
      return Node.Status.Failure;

    Vector2 top = _agent.GetOwnGoalTop();
    Vector2 bottom = _agent.GetOwnGoalBottom();
    Vector2 A = top;
    Vector2 B = bottom;
    Vector2 C = ball;
    Vector2 D = (Vector2)closestOpp.transform.position;

    float m = (B.y - A.y) / (B.x - A.x);

    float yInt = A.y + (C.x - A.x) * m;

    if (yInt >= Mathf.Min(A.y, B.y) && yInt <= Mathf.Max(A.y, B.y))
      return Node.Status.Success;

    return Node.Status.Failure;
  }

  public void Reset() {}
}
}