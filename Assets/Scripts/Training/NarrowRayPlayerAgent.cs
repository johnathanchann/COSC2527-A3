using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(DecisionRequester))]
public class NarrowRayPlayerAgent : Agent
{
    private Player _player;
    private float _sign;

    public override void OnEpisodeBegin()
    {
        _player = GetComponent<Player>();
        _sign = _player.team == Team.Left ? 1f : -1f;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int act = actions.DiscreteActions[0];
        Vector2 dir = Vector2.zero;
        bool kick = false;

        switch (act)
        {
            case 0: // do nothing
                break;
            case 1: // kick
                kick = true;
                break;
            case 2: dir = new Vector2(0, 1); break; // up
            case 3: dir = new Vector2(0, -1); break; // down
            case 4: dir = new Vector2(-1, 0); break; // left
            case 5: dir = new Vector2(1, 0); break; // right
            case 6: dir = new Vector2(-1, 1); break; // up-left
            case 7: dir = new Vector2(1, 1); break; // up-right
            case 8: dir = new Vector2(-1, -1); break; // down-left
            case 9: dir = new Vector2(1, -1); break; // down-right
        }
        dir = dir.normalized * _sign;
        _player.ApplyAction(dir, kick);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var d = actionsOut.DiscreteActions;
        d[0] = 0;
        if (Input.GetKey(KeyCode.Space))
        {
            d[0] = 1;
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (h == 0 && v == 0) return;

        if (v > 0 && h == 0) d[0] = 2;
        else if (v < 0 && h == 0) d[0] = 3;
        else if (h < 0 && v == 0) d[0] = 4;
        else if (h > 0 && v == 0) d[0] = 5;
        else if (h < 0 && v > 0) d[0] = 6;
        else if (h > 0 && v > 0) d[0] = 7;
        else if (h < 0 && v < 0) d[0] = 8;
        else if (h > 0 && v < 0) d[0] = 9;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            AddReward(0.01f);
        }
    }

}