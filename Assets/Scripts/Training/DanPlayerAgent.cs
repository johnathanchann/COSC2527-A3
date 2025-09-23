using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;
using System.Linq;
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(DecisionRequester))]
public class DanPlayerAgent : Agent
{
    [Header("References")]
    public Player _opponentPlayer;
    public Player _player;
    private Rigidbody2D _playerRb;
    private Rigidbody2D _ballRb;
    private Transform _ballT;
    public GameManager GameManager;
    public SpriteRenderer Field;
    private float _sign;
    public BoxCollider2D OwnGoal;
    public BoxCollider2D OpponentGoal;

    private float _fieldHalfWidth;
    private float _fieldHalfHeight;
    private float _fieldHeight;
    private float _fieldWidth;
    private float _fieldDiagonal;

    public override void OnEpisodeBegin()
    {
        // Initialize player and ball refs
        _player = GetComponent<Player>();
        _playerRb = GetComponent<Rigidbody2D>();
        _ballT = _player.ball.transform;
        _ballRb = _player.ball.GetComponent<Rigidbody2D>();
        _sign = _player.team == Team.Left ? 1f : -1f;

        if (Field != null)
        {
            Bounds b = Field.bounds;
            _fieldWidth = b.size.x;
            _fieldHeight = b.size.y;
            _fieldHalfWidth = b.extents.x;
            _fieldHalfHeight = b.extents.y;
            float width = b.size.x;
            float height = b.size.y;
            _fieldDiagonal = Mathf.Sqrt(width * width + height * height);
        }
        else
        {
            Debug.LogWarning("Field SpriteRenderer is not assigned! Normalization will be invalid.");
        }
    }

    private Vector2 GetPosition() => _sign * (Vector2)transform.localPosition;
    private Vector2 GetBallPosition() => _sign * (Vector2)_ballT.localPosition;
    private Vector2 GetOpponentPosition() => _sign * (Vector2)_opponentPlayer.transform.localPosition;
    private Vector2 GetPlayerVelocity() => _sign * _playerRb.linearVelocity;
    private Vector2 GetOpponentVelocity() => _sign * _opponentPlayer.GetComponent<Rigidbody2D>().linearVelocity;
    private Vector2 GetBallVelocity() => _sign * _ballRb.linearVelocity;
    private float GetBallOwnGoalDistance() => (OwnGoal.ClosestPoint(_ballT.position) - (Vector2)_ballT.position).magnitude;
    private float GetBallOpponentGoalDistance() => (OpponentGoal.ClosestPoint(_ballT.position) - (Vector2)_ballT.position).magnitude;

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector2 selfPos = GetPosition();
        Vector2 oppPos = GetOpponentPosition();
        Vector2 ballPos = GetBallPosition();
        float ownDist = GetBallOwnGoalDistance();
        float oppDist = GetBallOpponentGoalDistance();
        Vector2 selfVel = GetPlayerVelocity();
        Vector2 oppVel = GetOpponentVelocity();
        Vector2 ballVel = GetBallVelocity();
        Vector2 ballRel = ballPos - selfPos;
        Vector2 oppRel = oppPos - selfPos;

        var obs = new List<float>()
        {
            // positions
            selfPos.x / _fieldHalfWidth,
            selfPos.y / _fieldHalfHeight,
            oppPos.x  / _fieldHalfWidth,
            oppPos.y  / _fieldHalfHeight,
            ballPos.x / _fieldHalfWidth,
            ballPos.y / _fieldHalfHeight,

            // goal distances
            ownDist / _fieldDiagonal,
            oppDist / _fieldDiagonal,

            // self velocity (dir + speed)
            selfVel.normalized.x,
            selfVel.normalized.y,
            selfVel.x / _player.NormalSpeed,
            selfVel.y / _player.NormalSpeed,
            selfVel.magnitude / _player.NormalSpeed,

            // opponent velocity
            oppVel.normalized.x,
            oppVel.normalized.y,
            oppVel.x/ _player.NormalSpeed,
            oppVel.y / _player.NormalSpeed,
            oppVel.magnitude / _player.NormalSpeed,

            // ball velocity
            ballVel.normalized.x,
            ballVel.normalized.y,
            ballVel.x / _player.KickPower,
            ballVel.y / _player.KickPower,
            ballVel.magnitude / _player.KickPower,

            // relative positions (full field width/height)
            ballRel.x / _fieldWidth,
            ballRel.y / _fieldHeight,
            ballRel.normalized.x,
            ballRel.normalized.y,

            oppRel.x  / _fieldWidth,
            oppRel.y  / _fieldHeight,
            oppRel.normalized.x,
            oppRel.normalized.y,


            // relative distances
            ballRel.magnitude / _fieldDiagonal,
            oppRel.magnitude  / _fieldDiagonal,
        };

        foreach (var f in obs)
            sensor.AddObservation(f);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        int direction = actions.DiscreteActions[0];
        bool kick = actions.DiscreteActions[1] == 1;
        Vector2 dir = Vector2.zero;

        switch (direction)
        {
            case 1: dir = Vector2.up; break;
            case 2: dir = Vector2.down; break;
            case 3: dir = Vector2.left; break;
            case 4: dir = Vector2.right; break;
            case 5: dir = Vector2.up + Vector2.left; break;
            case 6: dir = Vector2.up + Vector2.right; break;
            case 7: dir = Vector2.down + Vector2.left; break;
            case 8: dir = Vector2.down + Vector2.right; break;
        }
        dir = dir.normalized * _sign;
        _player.ApplyAction(dir, kick);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var d = actionsOut.DiscreteActions;
        d[0] = 0; d[1] = 0;
        if (Input.GetKey(KeyCode.Space)) d[1] = 1;
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (h != 0 || v != 0)
        {
            if (v > 0 && h == 0) d[0] = 1;
            else if (v < 0 && h == 0) d[0] = 2;
            else if (h < 0 && v == 0) d[0] = 3;
            else if (h > 0 && v == 0) d[0] = 4;
            else if (h < 0 && v > 0) d[0] = 5;
            else if (h > 0 && v > 0) d[0] = 6;
            else if (h < 0 && v < 0) d[0] = 7;
            else if (h > 0 && v < 0) d[0] = 8;
        }
    }
}
