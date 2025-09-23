using UnityEngine;
using Unity.MLAgents;

public class SelfPlayManager : MonoBehaviour
{
    [Header("Agents & Ball")]
    public Agent LeftAgent;
    public Agent RightAgent;
    public int MaxEnvironmentSteps = 3000;
    public GameManager GameManager;

    public BoxCollider2D RandomSpawnZone;
    public Transform BallSpawn;
    public Transform Player1Spawn;
    public Transform Player2Spawn;

    public bool RandomSpawn = false;
    public bool RandomSide = false;

    private int m_ResetTimer = 0;

    private bool learnSoccerSkill = false;
    private int currentTrainingSide = 0;// 0 for left, 1 for right
    private int leftTeamScore = 0;
    private int rightTeamScore = 0;
    private bool endEpisodeOnGoal = true;

    public void Start()
    {
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
    }
    public void Reset()
    {
        m_ResetTimer = 0;
        leftTeamScore = 0;
        rightTeamScore = 0;
        ResetField();
    }
    public void EndEpisode()
    {
        LeftAgent.EndEpisode();
        RightAgent.EndEpisode();
        Reset();
    }

    public void InteruptEpisode()
    {
        if (!endEpisodeOnGoal)
        {
            if (leftTeamScore > rightTeamScore)
            {
                LeftAgent.AddReward(0.5f);
                RightAgent.AddReward(-0.5f);
            }
            else if (rightTeamScore > leftTeamScore)
            {
                LeftAgent.AddReward(-0.5f);
                RightAgent.AddReward(0.5f);
            }
        }
        LeftAgent.EpisodeInterrupted();
        RightAgent.EpisodeInterrupted();
        Reset();
    }

    public void ResetField()
    {
        if (RandomSide)
        {
            currentTrainingSide = Random.Range(0, 2);
            if (currentTrainingSide == 0)
            {
                ((SoccerAgent)LeftAgent).ChangeSide(Team.Left);
                ((SoccerAgent)RightAgent).ChangeSide(Team.Right);
            }
            else
            {
                ((SoccerAgent)LeftAgent).ChangeSide(Team.Right);
                ((SoccerAgent)RightAgent).ChangeSide(Team.Left);
            }
        }
        if (RandomSpawn)
        {
            Player1Spawn.position = GetRandomPointInZone();
            Player2Spawn.position = GetRandomPointInZone();
            BallSpawn.position = GetRandomPointInZone();
        }
        GameManager.Refresh();
    }

    public void ReportGoal(int scoringTeamID)
    {
        if (learnSoccerSkill)
        {
            if (scoringTeamID == currentTrainingSide)
            {
                print("Goal scored by the training team!");
                LeftAgent.AddReward(1f * 1000);
                RightAgent.AddReward(-1f * 0);
                ResetField();
            }
            else
            {
                LeftAgent.AddReward(-1f * 0);
                RightAgent.AddReward(1f * 1000);
                EndEpisode();
            }
            return;
        }

        float bonus = 1f - m_ResetTimer / MaxEnvironmentSteps;
        //float bonus = 0;
        if (scoringTeamID == 0)
        {
            print("Left team scored a goal!");
            LeftAgent.AddReward(1f + bonus);
            RightAgent.AddReward(-1f);
        }
        else
        {
            print("Right team scored a goal!");
            LeftAgent.AddReward(-1f);
            RightAgent.AddReward(1f + bonus);
        }
        if (endEpisodeOnGoal)
        {
            EndEpisode();
        }
        else
        {
            ResetField();
        }
    }
}
