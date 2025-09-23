using UnityEngine;
using Unity.MLAgents;
using System.Collections.Generic;

public class SelfPlayManagerMulti : MonoBehaviour
{
    [Header("Agents & Ball")]
    public List<PlayerAgentMulti> AgentsList = new List<PlayerAgentMulti>();
    public int MaxEnvironmentSteps = 3000;
    public GameManagerMulti GameManager;
    private SimpleMultiAgentGroup m_LeftAgentGroup;
    private SimpleMultiAgentGroup m_RightAgentGroup;

    public BoxCollider2D RandomSpawnZone;
    public Transform BallSpawn;
    public TeamGroup LeftTeam;
    public TeamGroup RightTeam;
    public bool RandomSpawn = false;

    private int m_ResetTimer = 0;


    public void Start()
    {
        m_LeftAgentGroup = new SimpleMultiAgentGroup();
        m_RightAgentGroup = new SimpleMultiAgentGroup();
        int leftCount = 0;
        int rightCount = 0;

        for (int i = 0; i < AgentsList.Count; i++)
        {
            if (AgentsList[i].team == Team.Left)
            {
                m_LeftAgentGroup.RegisterAgent(AgentsList[i]);
                Debug.Log($"[SelfPlayManager] Registered LEFT agent: {AgentsList[i].name}");
                leftCount++;
            }
            else
            {
                m_RightAgentGroup.RegisterAgent(AgentsList[i]);
                Debug.Log($"[SelfPlayManager] Registered RIGHT agent: {AgentsList[i].name}");
                rightCount++;
            }
        }

        if (leftCount != rightCount)
        {
            Debug.LogError($"Team size mismatch: Left = {leftCount}, Right = {rightCount}");
            return;
        }
        else
        {
            Debug.Log($"Teams initialized: Left = {leftCount}, Right = {rightCount}");
        }
        Reset();
    }
    private Vector2 GetRandomPointInZone()
    {
        Bounds bounds = RandomSpawnZone.bounds; // This gives correct world-space box with scale & offset
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }

    public void FixedUpdate()
    {
        m_ResetTimer += 1;

        if (m_ResetTimer >= MaxEnvironmentSteps)
        {
            InteruptEpisode();
        }
        // bool leftTeamHoldBall = false;
        // bool rightTeamHoldBall = false;
        // foreach (var agent in AgentsList)
        // {
        //     if (agent._player.CanShoot() && agent.team == Team.Left)
        //     {
        //         leftTeamHoldBall = true;
        //     }
        //     if (agent._player.CanShoot() && agent.team == Team.Right)
        //     {
        //         rightTeamHoldBall = true;
        //     }
        // }
        // if (leftTeamHoldBall)
        // {
        //     m_LeftAgentGroup.AddGroupReward(0.0001f);
        //     m_RightAgentGroup.AddGroupReward(-0.0001f);
        // }
        // if (rightTeamHoldBall)
        // {
        //     m_RightAgentGroup.AddGroupReward(0.0001f);
        //     m_LeftAgentGroup.AddGroupReward(-0.0001f);
        // }
    }
    public void Reset()
    {
        m_ResetTimer = 0;
        if (RandomSpawn)
        {
            for (int i = 0; i < LeftTeam.Players.Length; i++)
            {
                LeftTeam.Players[i].SpawnPoint.transform.position = GetRandomPointInZone();
                RightTeam.Players[i].SpawnPoint.transform.position = GetRandomPointInZone();
            }
            BallSpawn.position = GetRandomPointInZone();
        }
        GameManager.Refresh(RandomSpawn);
    }
    public void EndEpisode()
    {
        m_LeftAgentGroup.EndGroupEpisode();
        m_RightAgentGroup.EndGroupEpisode();
        Reset();
    }

    public void InteruptEpisode()
    {
        m_LeftAgentGroup.GroupEpisodeInterrupted();
        m_RightAgentGroup.GroupEpisodeInterrupted();
        Reset();
    }

    public void ReportGoal(int scoringTeamID)
    {
        if (scoringTeamID == 0)
        {
            Debug.Log("[SelfPlayManager] Left team scored!");
            m_LeftAgentGroup.AddGroupReward(+1f);
            m_RightAgentGroup.AddGroupReward(-1f);
        }
        else
        {
            Debug.Log("[SelfPlayManager] Right team scored!");
            m_LeftAgentGroup.AddGroupReward(-1f);
            m_RightAgentGroup.AddGroupReward(+1f);
        }
        EndEpisode();
    }
}
