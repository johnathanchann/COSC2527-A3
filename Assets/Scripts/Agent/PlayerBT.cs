using UnityEngine;
using BehavorialTree;

[RequireComponent(typeof(Player))]
public class PlayerBt : AgentBase {
  [Header("References")]
  public Player opponentPlayer;
  public GameManager GameManager;

  // Cached components
  private Player _player;
  private Rigidbody2D _playerRb, _ballRb, _oppRb;
  private Transform _ballT;
  private float _sign;

  [Header("Outputs (set by actions)")]
  public Vector2 DesiredDirection { get; set; }
  public KickType DesiredKick { get; set; }

  [Header("Settings")]
  public LayerMask obstacleMask;
  public float KickDistance = 1f;
  public float FieldHalfWidth = 9f;
  public float GoalHalfHeight = 2f;
  public float PossessionCooldown = 0.3f;
  public float BlockAfterLeaveTime = 0.3f;

  private float _lastPossessionLossTime = -10f;

  private BehaviourTree _bt;

  protected override void Start() {
    base.Start();

    _player = GetComponent<Player>();
    _playerRb = _player.GetComponent<Rigidbody2D>();
    _ballT = _player.ball.transform;
    _ballRb = _player.ball.GetComponent<Rigidbody2D>();
    _sign = _player.team == Team.Left ? 1f : -1f;
    if (opponentPlayer)
      _oppRb = opponentPlayer.GetComponent<Rigidbody2D>();

    BuildBehaviorTree();
  }

  private void BuildBehaviorTree() {

    _bt = new BehaviourTree("UltimatePlayerBT");

    var root = new Selector("Root");

    var cornerSeq = new Sequence("CornerShot");
    cornerSeq.AddChild(new ActionNode("BallInCorner", new BallInCorner(this)));
    cornerSeq.AddChild(new ActionNode("CloseToBall", new CloseToBall(this)));
    cornerSeq.AddChild(
        new ActionNode("ShootFromCorner", new ShootFromCorner(this)));
    root.AddChild(cornerSeq);

    var openShot = new Sequence("OpenShot");
    openShot.AddChild(new ActionNode(
        "BallPossession", new BallPossession(this, (int)_player.team, true)));
    openShot.AddChild(
        new ActionNode("ClearPathToGoal", new ClearPathToGoalDaemon(this)));
    openShot.AddChild(new ActionNode("Align", new Align(this)));
    openShot.AddChild(new ActionNode("Shoot", new Shoot(this)));
    root.AddChild(openShot);

    var quickShot = new Sequence("QuickShot");
    quickShot.AddChild(new ActionNode(
        "BallPossess", new BallPossession(this, (int)_player.team, true)));
    quickShot.AddChild(new ActionNode("JustShoot", new JustShoot(this)));
    root.AddChild(quickShot);

    var wallShot = new Sequence("WallShot");
    wallShot.AddChild(new ActionNode(
        "BallOwned", new BallPossession(this, (int)_player.team, true)));
    wallShot.AddChild(new ActionNode("ShootAtWall", new ShootAtWall(this)));
    root.AddChild(wallShot);

    var advanceSel = new Selector("Advance");
    var advanceField = new Sequence("AdvanceField");
    advanceField.AddChild(
        new ActionNode("SideOfField", new SideOfTheFieldDaemon(this, true)));
    advanceField.AddChild(
        new ActionNode("MoveToOppSide", new MoveToOpponentSideAction(this)));
    advanceSel.AddChild(advanceField);
    var driveGoal = new Sequence("DriveGoal");
    driveGoal.AddChild(
        new ActionNode("MoveTowardsGoal", new MoveTowardsOpponentGoal(this)));
    driveGoal.AddChild(new ActionNode("FaceGoal", new FaceOpponentGoal(this)));
    advanceSel.AddChild(driveGoal);
    root.AddChild(advanceSel);

    var defendSel = new Selector("Defend");
    var blockSeq = new Sequence("BlockShot");
    blockSeq.AddChild(
        new ActionNode("DangerToGoal", new DangerToGoalDaemon(this, 5f)));
    blockSeq.AddChild(
        new ActionNode("SmartAlign", new SmartAlignToDefend(this)));
    blockSeq.AddChild(
        new ActionNode("PredictKick", new PredictCounterKick(this)));
    defendSel.AddChild(blockSeq);
    var guardSeq = new Sequence("GuardGoal");
    guardSeq.AddChild(
        new ActionNode("DangerNearGoal", new DangerToGoalDaemon(this, 5f)));
    guardSeq.AddChild(
        new ActionNode("MoveMiddleGoal", new MoveMiddleOfGoal(this)));
    defendSel.AddChild(guardSeq);
    defendSel.AddChild(new ActionNode("MoveBetweenOppGoal",
                                      new MoveBetweenOpponentAndGoal(this)));
    root.AddChild(defendSel);

    var interceptSeq = new Sequence("Intercept");
    interceptSeq.AddChild(new ActionNode("BallMoving", new BallMoving(this)));
    interceptSeq.AddChild(
        new ActionNode("InterceptBall", new InterceptBall(this)));
    root.AddChild(interceptSeq);

    var clearSeq = new Sequence("ClearBall");
    clearSeq.AddChild(new ActionNode("RetreatToGetPossession",
                                     new RetreatToGetPossession(this)));
    clearSeq.AddChild(
        new ActionNode("KickBallAway", new KickBallAway(this, KickDistance)));
    root.AddChild(clearSeq);

    var chaseSeq = new Sequence("ChaseOpponent");
    chaseSeq.AddChild(new ActionNode(
        "DistanceFromOpp",
        new DistanceFromOpponent(this, (int)_player.team, 100f)));
    chaseSeq.AddChild(new ActionNode("Chase", new Chase(this)));
    root.AddChild(chaseSeq);

    var possessSeq = new Sequence("TakePossession");
    possessSeq.AddChild(new ActionNode(
        "ClosestTeamToBall", new ClosestTeamToBall(this, (int)_player.team)));
    possessSeq.AddChild(new ActionNode("GoToBall", new GoToBall(this)));
    root.AddChild(possessSeq);

    root.AddChild(new ActionNode("GoToBallFallback", new GoToBall(this)));
    root.AddChild(new ActionNode("RandomMove", new RandomMove(this)));

    _bt.AddChild(root);
  }

  private void Update() {
    
  }

    public override void FixedUpdate()
    {
        _bt.Reset();
        var status = _bt.Process();
        string path = _bt.GetActivePath();
        Debug.Log($"BT tick => {status} | node: {path}");
        base.FixedUpdate();
    }

    public override (Vector2 dir, KickType kick) GetAction() {
    var outp = (DesiredDirection, DesiredKick);
    DesiredDirection = Vector2.zero;
    DesiredKick = KickType.None;
    return outp;
  }

  public Vector2 GetPlayerWorldPosition() => _player.transform.position;
  public Vector2 GetBallWorldPosition() => _ballT.position;
  public Vector2 GetOpponentWorldPosition() =>
      opponentPlayer ? opponentPlayer.transform.position : Vector2.zero;
  public Vector2 GetPlayerWorldVelocity() => _playerRb.linearVelocity;
  public Vector2 GetOpponentWorldVelocity() => _oppRb ? _oppRb.linearVelocity
                                                      : Vector2.zero;
  public Vector2 GetBallWorldVelocity() => _ballRb.linearVelocity;
  public float BallRadius => _player.KickRadius;
  public Team Team => _player.team;
  public Rigidbody2D GetBallRigidbody() => _ballRb;
  public Rigidbody2D GetOpponentRigidbody() => _oppRb;
  public void RecordPossessionLoss() => _lastPossessionLossTime = Time.time;

  public Vector2 GetOpponentGoalTop() => new Vector2(_sign * FieldHalfWidth,
                                                     GoalHalfHeight);
  public Vector2 GetOpponentGoalBottom() => new Vector2(_sign * FieldHalfWidth,
                                                        -GoalHalfHeight);
  public Vector2 GetOwnGoalTop() => new Vector2(-_sign * FieldHalfWidth,
                                                GoalHalfHeight);
  public Vector2 GetOwnGoalBottom() => new Vector2(-_sign * FieldHalfWidth,
                                                   -GoalHalfHeight);
}
