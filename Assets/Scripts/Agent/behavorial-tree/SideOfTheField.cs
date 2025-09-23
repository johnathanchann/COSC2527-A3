

using UnityEngine;

namespace BehavorialTree {

public class SideOfTheFieldDaemon : IStrategy {
  private readonly PlayerBt _agent;
  private readonly bool _ourSide;

  public SideOfTheFieldDaemon(PlayerBt agent, bool ourSide) {
    _agent = agent;
    _ourSide = ourSide;
  }

  public Node.Status Process() {
    float px = _agent.GetPlayerWorldPosition().x;
    if (_ourSide) {
      if (_agent.Team == Team.Left && px > 0)
        return Node.Status.Success;
      if (_agent.Team == Team.Right && px < 0)
        return Node.Status.Success;
    } else {
      if (_agent.Team == Team.Left && px < 0)
        return Node.Status.Success;
      if (_agent.Team == Team.Right && px > 0)
        return Node.Status.Success;
    }
    return Node.Status.Failure;
  }

  public void Reset() {}
}
}
