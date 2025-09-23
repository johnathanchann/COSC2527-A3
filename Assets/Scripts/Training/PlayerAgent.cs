using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(DecisionRequester))]
public class PlayerAgent : Agent
{
    [Header("References")]
    public Player _opponentPlayer;
    private Player _player;
    private Rigidbody2D _playerRb;
    private Rigidbody2D _ballRb;
    private Transform _ballT;
    public GameManager GameManager;
    private float _sign;


    public override void OnEpisodeBegin()
    {
        _player = GetComponent<Player>();
        _playerRb = GetComponent<Rigidbody2D>();
        _ballT = _player.ball.transform;
        _ballRb = _player.ball.GetComponent<Rigidbody2D>();
        _sign = _player.team == Team.Left ? 1f : -1f;
    }

    private Vector2 GetPosition()
    {
        return _sign * transform.localPosition;
    }

    private Vector2 GetBallPosition()
    {
        return _sign * _ballT.localPosition;
    }

    private Vector2 GetOpponentPosition()
    {
        return _sign * _opponentPlayer.transform.localPosition;
    }

    private Vector2 GetPlayerVelocity()
    {
        return _sign * _playerRb.linearVelocity;
    }

    private Vector2 GetOpponentVelocity()
    {
        return _sign * _opponentPlayer.GetComponent<Rigidbody2D>().linearVelocity;
    }

    private Vector2 GetBallVelocity()
    {
        return _sign * _ballRb.linearVelocity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var selfPos = GetPosition();
        sensor.AddObservation(selfPos.x);
        sensor.AddObservation(selfPos.y);

        var oppPos = GetOpponentPosition();
        sensor.AddObservation(oppPos.x);
        sensor.AddObservation(oppPos.y);

        var ballPos = GetBallPosition();
        sensor.AddObservation(ballPos.x);
        sensor.AddObservation(ballPos.y);

        var selfVel = GetPlayerVelocity();
        sensor.AddObservation(selfVel.x);
        sensor.AddObservation(selfVel.y);

        var oppVel = GetOpponentVelocity();
        sensor.AddObservation(oppVel.x);
        sensor.AddObservation(oppVel.y);

        var ballVel = GetBallVelocity();
        sensor.AddObservation(ballVel.x);
        sensor.AddObservation(ballVel.y);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int direction = actions.DiscreteActions[0]; // movement direction
        int kickFlag = actions.DiscreteActions[1];  // 0 = no kick, 1 = kick

        Vector2 dir = Vector2.zero;
        bool kick = kickFlag == 1;

        switch (direction)
        {
            case 0: break;
            case 1: dir = new Vector2(0, 1); break;
            case 2: dir = new Vector2(0, -1); break;
            case 3: dir = new Vector2(-1, 0); break;
            case 4: dir = new Vector2(1, 0); break;
            case 5: dir = new Vector2(-1, 1); break;
            case 6: dir = new Vector2(1, 1); break;
            case 7: dir = new Vector2(-1, -1); break;
            case 8: dir = new Vector2(1, -1); break;
            case 9: dir = Vector2.zero; break;
        }

        dir = dir.normalized * _sign;
        _player.ApplyAction(dir, kick);
    }


    void OnDrawGizmos()
    {

        if (_player == null || _ballT == null || _ballRb == null || _opponentPlayer == null)
            return;

        Gizmos.color = Color.green;
        Vector2 playerPos = _sign * GetPosition();
        Gizmos.DrawSphere(playerPos, 0.1f);

        Gizmos.color = Color.yellow;
        Vector2 oppPos = _sign * GetOpponentPosition();
        Gizmos.DrawSphere(oppPos, 0.1f);

        Gizmos.color = Color.blue;
        Vector2 ballPos = _sign * GetBallPosition();
        Gizmos.DrawSphere(ballPos, 0.1f);

        Gizmos.color = Color.red;
        Vector2 ballVel = _sign * GetBallVelocity();
        Vector2 dir = ballVel.normalized;
        float arrowLength = ballVel.magnitude * 0.1f;

        Gizmos.DrawLine(ballPos, ballPos + dir * arrowLength);

        Vector2 right = Quaternion.Euler(0, 0, 150) * dir;
        Vector2 left = Quaternion.Euler(0, 0, 210) * dir;
        Gizmos.DrawLine(ballPos + dir * arrowLength, ballPos + dir * arrowLength + right * 0.1f);
        Gizmos.DrawLine(ballPos + dir * arrowLength, ballPos + dir * arrowLength + left * 0.1f);
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var d = actionsOut.DiscreteActions;
        d[0] = 0; // direction
        d[1] = 0; // kick

        if (Input.GetKey(KeyCode.Space))
            d[1] = 1;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h == 0 && v == 0) return;

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