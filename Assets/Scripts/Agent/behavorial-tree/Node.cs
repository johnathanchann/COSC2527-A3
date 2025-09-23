using System.Collections.Generic;
using UnityEngine;

namespace BehavorialTree {

public abstract class Node {
  public enum Status { Success, Failure, Running }

  public string Name { get; }
  protected List<Node> children = new List<Node>();
  protected int currentChild = 0;

  public IReadOnlyList<Node> Children => children;
  public int CurrentChild => currentChild;

  public virtual string GetActivePath() {
    if (children.Count == 0 || currentChild >= children.Count)
      return Name;
    return Name + "/" + children[currentChild].GetActivePath();
  }

  protected Node(string name) { Name = name; }

  public abstract Status Process();

  public virtual void Reset() {
    currentChild = 0;
    foreach (var child in children)
      child.Reset();
  }

  public void AddChild(Node child) { children.Add(child); }
}

public class Selector : Node {
  public Selector(string name) : base(name) {}

  public override Status Process() {
    Debug.Log($"Selector {Name} processing child index {currentChild}");
    while (currentChild < children.Count) {
      var status = children[currentChild].Process();
      Debug.Log(
          $"Selector {Name} -> {children[currentChild].Name} => {status}");
      if (status == Status.Running)
        return Status.Running;
      if (status == Status.Success) {
        Reset();
        return Status.Success;
      }
      currentChild++;
    }
    Reset();
    return Status.Failure;
  }
}

public class Sequence : Node {
  public Sequence(string name) : base(name) {}

  public override Status Process() {
    Debug.Log($"Sequence {Name} processing child index {currentChild}");
    while (currentChild < children.Count) {
      var status = children[currentChild].Process();
      Debug.Log(
          $"Sequence {Name} -> {children[currentChild].Name} => {status}");
      if (status == Status.Running)
        return Status.Running;
      if (status == Status.Failure) {
        Reset();
        return Status.Failure;
      }
      currentChild++;
    }
    Reset();
    return Status.Success;
  }
}

public class BehaviourTree : Node {
  public BehaviourTree(string name) : base(name) {}

  public override Status Process() {
    Debug.Log($"BehaviourTree {Name} processing child index {currentChild}");
    while (currentChild < children.Count) {
      var status = children[currentChild].Process();
      Debug.Log(
          $"BehaviourTree {Name} -> {children[currentChild].Name} => {status}");
      if (status == Status.Running)
        return Status.Running;
      if (status == Status.Failure) {
        Reset();
        return Status.Failure;
      }
      currentChild++;
    }
    Reset();
    return Status.Success;
  }
}

public class ActionNode : Node {
  private readonly IStrategy strategy;

  public ActionNode(string name, IStrategy strategy) : base(name) {
    this.strategy = strategy;
  }

  public override Status Process() {
    Debug.Log($"ActionNode {Name} executing");
    var result = strategy.Process();
    Debug.Log($"ActionNode {Name} => {result}");
    if (result != Status.Running)
      strategy.Reset();
    return result;
  }
}
}
