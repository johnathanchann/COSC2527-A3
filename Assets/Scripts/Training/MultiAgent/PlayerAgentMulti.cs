using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Linq;
using System.Collections.Generic;
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(DecisionRequester))]
public class PlayerAgentMulti : Agent
{
    [Header("References")]
    public TeamGroup teamGroup;
    public TeamGroup opponentTeamGroup;
    public Player _player;
    private Rigidbody2D _playerRb;
    private Rigidbody2D _ballRb;
    private Transform _ballT;
    private float _sign;
    public Team team;
    public SpriteRenderer Field;

    private float _fieldWidth;
    private float _fieldHeight;
    private float _fieldHalfWidth;
    private float _fieldHalfHeight;
    private float _fieldDiagonal;
    public override void OnEpisodeBegin()
    {
        _player = GetComponent<Player>();
        _playerRb = GetComponent<Rigidbody2D>();
        _ballT = _player.ball.transform;
        _ballRb = _player.ball.GetComponent<Rigidbody2D>();
        _sign = _player.team == Team.Left ? 1f : -1f;
        team = _player.team;
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
            Debug.Log($"Field dimensions: Width = {_fieldWidth}, Height = {_fieldHeight}, Diagonal = {_fieldDiagonal}");
        }
        else
        {
            Debug.LogWarning("Field SpriteRenderer is not assigned! Normalization will be invalid.");
        }
    }

    private Vector2 GetPosition()
    {
        return _sign * transform.localPosition;
    }

    private Vector2 GetBallPosition()
    {
        return _sign * _ballT.localPosition;
    }

    private Vector2 GetPlayerVelocity()
    {
        return _sign * _playerRb.linearVelocity;
    }

    private Vector2 GetBallVelocity()
    {
        return _sign * _ballRb.linearVelocity;
    }
    private float GetBallDistanceFromOwnGoal()
    {
        return (teamGroup.OwnGoalZone.ClosestPoint(_ballT.position) - (Vector2)_ballT.position).magnitude;
    }
    private float GetBallDistanceFromOpponentGoal()
    {
        return (opponentTeamGroup.OpponentGoalZone.ClosestPoint(_ballT.position) - (Vector2)_ballT.position).magnitude;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Self and ball features
        var selfPos = GetPosition();
        sensor.AddObservation(GetBallDistanceFromOwnGoal() / _fieldDiagonal);
        sensor.AddObservation(GetBallDistanceFromOpponentGoal() / _fieldDiagonal);
        sensor.AddObservation(selfPos.x / _fieldHalfWidth);
        sensor.AddObservation(selfPos.y / _fieldHalfHeight);

        var selfVel = GetPlayerVelocity();
        sensor.AddObservation(selfVel / _player.NormalSpeed);
        sensor.AddObservation(selfVel.normalized);
        sensor.AddObservation(selfVel.magnitude / _player.NormalSpeed);

        var ballPos = GetBallPosition();
        sensor.AddObservation(ballPos.x / _fieldHalfWidth);
        sensor.AddObservation(ballPos.y / _fieldHalfHeight);

        var ballVel = GetBallVelocity();
        sensor.AddObservation(ballVel / _player.KickPower);
        sensor.AddObservation(ballVel.normalized);
        sensor.AddObservation(ballVel.magnitude / _player.KickPower);

        var relBall = ballPos - selfPos;
        sensor.AddObservation(relBall.x / _fieldWidth);
        sensor.AddObservation(relBall.y / _fieldHeight);
        sensor.AddObservation(relBall.normalized);
        sensor.AddObservation(relBall.magnitude / _fieldDiagonal);

        // Collect, sort, and add opponent observations
        var opponentList = new List<(float angle, float dist, float[] feats)>();
        foreach (var op in opponentTeamGroup.Players)
        {
            var pos = _sign * (Vector2)op.transform.localPosition;
            var vel = _sign * op.GetComponent<Rigidbody2D>().linearVelocity;
            var rel = pos - selfPos;
            var angle = Mathf.Atan2(rel.y, rel.x);
            if (angle < 0) angle += 2 * Mathf.PI;
            var dist = rel.magnitude;

            var feats = new List<float> {
                pos.x / _fieldHalfWidth,
                pos.y / _fieldHalfHeight,
                vel.x / _player.NormalSpeed,
                vel.y / _player.NormalSpeed,
                vel.normalized.x,
                vel.normalized.y,
                vel.magnitude / _player.NormalSpeed,
                rel.x / _fieldWidth,
                rel.y / _fieldHeight,
                rel.normalized.x,
                rel.normalized.y,
                rel.magnitude / _fieldDiagonal
            }.ToArray();

            opponentList.Add((angle, dist, feats));
        }
        opponentList.Sort((a, b) =>
        {
            var c = a.angle.CompareTo(b.angle);
            return c != 0 ? c : a.dist.CompareTo(b.dist);
        });
        foreach (var entry in opponentList)
            foreach (var f in entry.feats)
                sensor.AddObservation(f);

        // Collect, sort, and add teammate observations
        var teammateList = new List<(float angle, float dist, float[] feats)>();
        foreach (var tm in teamGroup.Players)
        {
            if (tm == _player) continue;
            var pos = _sign * (Vector2)tm.transform.localPosition;
            var vel = _sign * tm.GetComponent<Rigidbody2D>().linearVelocity;
            var rel = pos - selfPos;
            var angle = Mathf.Atan2(rel.y, rel.x);
            if (angle < 0) angle += 2 * Mathf.PI;
            var dist = rel.magnitude;

            var feats = new List<float> {
                pos.x / _fieldHalfWidth,
                pos.y / _fieldHalfHeight,
                vel.x / _player.NormalSpeed,
                vel.y / _player.NormalSpeed,
                vel.normalized.x,
                vel.normalized.y,
                vel.magnitude / _player.NormalSpeed,
                rel.x / _fieldWidth,
                rel.y / _fieldHeight,
                rel.normalized.x,
                rel.normalized.y,
                rel.magnitude / _fieldDiagonal
            }.ToArray();

            teammateList.Add((angle, dist, feats));
        }
        teammateList.Sort((a, b) =>
        {
            var c = a.angle.CompareTo(b.angle);
            return c != 0 ? c : a.dist.CompareTo(b.dist);
        });
        foreach (var entry in teammateList)
            foreach (var f in entry.feats)
                sensor.AddObservation(f);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveAction = actions.DiscreteActions[0];
        int kickAction = actions.DiscreteActions[1];

        Vector2 dir = Vector2.zero;
        bool kick = (kickAction == 1);

        switch (moveAction)
        {
            case 0: break; // no movement
            case 1: dir = new Vector2(0, 1); break;     // up
            case 2: dir = new Vector2(0, -1); break;    // down
            case 3: dir = new Vector2(-1, 0); break;    // left
            case 4: dir = new Vector2(1, 0); break;     // right
            case 5: dir = new Vector2(-1, 1); break;    // up-left
            case 6: dir = new Vector2(1, 1); break;     // up-right
            case 7: dir = new Vector2(-1, -1); break;   // down-left
            case 8: dir = new Vector2(1, -1); break;    // down-right
        }

        dir = dir.normalized * _sign;
        _player.ApplyAction(dir, kick);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var d = actionsOut.DiscreteActions;

        d[0] = 0;
        d[1] = 0;

        if (Input.GetKey(KeyCode.Space))
        {
            d[1] = 1;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

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