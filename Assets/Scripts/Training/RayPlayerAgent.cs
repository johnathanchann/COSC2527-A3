using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(DecisionRequester))]
public class RayPlayerAgent : Agent
{
    public Player _opponentPlayer;
    private Player _player;
    private Rigidbody2D _playerRb;
    private Rigidbody2D _ballRb;
    private Transform _ballT;
    private float _sign;
    private bool enableSubRewards = false;

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


        if (enableSubRewards)
        {
            // == Reward 2: Velocity toward ball ==
            //Vector2 toBall = (_ballT.position - transform.position).normalized;
            //float velocityTowardBall = Vector2.Dot(_playerRb.linearVelocity, toBall);
            // Only reward forward movement (not away from ball)
            //float directionalVelocity = Mathf.Max(0f, velocityTowardBall);
            //AddReward(directionalVelocity * 0.05f / 500);

            // == Reward 3: Forward velocity ==
            //AddReward(_playerRb.linearVelocity.magnitude * 0.1f / 1000);
            //print($"[{_player.name}] Reward for forward velocity: {_playerRb.linearVelocity.magnitude * 0.1f}");

            // == Reward 4: Ball possession ==
            float distToBall = Vector2.Distance(_player.transform.position, _ballT.position);
            float epsilon = 0.1f;
            float proximityReward = Mathf.Max(0, 1 - Mathf.Log(distToBall + epsilon));
            //float proximityReward = 1.5f - 0.5f * Mathf.Log(distToBall + epsilon);
            AddReward(proximityReward * 0.0005f);
            //print($"[{_player.name}] Reward for possession: {proximityReward * 0.001f}");
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!enableSubRewards) return;
        if (collision.gameObject.CompareTag("Ball"))
        {
            AddReward(0.01f);
            print($"{_player.name} collide with ball");
        }
    }

    void OnDrawGizmos()
    {

        if (_player == null || _ballT == null || _ballRb == null || _opponentPlayer == null)
            return;

        Gizmos.color = _player.team == Team.Left ? Color.green : Color.yellow;
        Vector2 playerPos = _sign * GetPosition();
        Gizmos.DrawSphere(playerPos, 0.1f);

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
}