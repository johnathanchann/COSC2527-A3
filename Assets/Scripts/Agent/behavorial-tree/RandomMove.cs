

using UnityEngine;

namespace BehavorialTree {

public class RandomMove : IStrategy {
  private readonly PlayerBt _agent;
  private Vector2 _currentDir;
  private float _timer;
  private const float Interval = 2.0f;

  public RandomMove(PlayerBt agent) {
    _agent = agent;
    _currentDir = Random.insideUnitCircle.normalized;
    _timer = Interval;
  }

  public Node.Status Process() {
    _timer -= Time.deltaTime;
    if (_timer <= 0f) {
      _currentDir = Random.insideUnitCircle.normalized;
      _timer = Interval;
    }

    _agent.DesiredDirection = _currentDir;
    _agent.DesiredKick = KickType.None;
    return Node.Status.Running;
  }

  public void Reset() { _timer = 0f; }
}
}
