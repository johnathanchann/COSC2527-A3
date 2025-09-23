using UnityEngine;
public class TeamGroup : MonoBehaviour
{
    public Player[] Players;
    public Team Team;
    public BoxCollider2D OpponentGoalZone;
    public BoxCollider2D OwnGoalZone;
    void Awake()
    {
        Players = GetComponentsInChildren<Player>();
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].team = Team;
        }
    }
    public void Refresh()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].Refresh();
        }
    }
}