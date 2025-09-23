using UnityEngine;

public class GoalZoneTrain : MonoBehaviour
{
    [Header("Which side is this?")]
    [SerializeField] bool isLeftGoal;

    public GameObject ball;
    public SelfPlayManager SelfPlayManager;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != ball) return;
        if (isLeftGoal)
            SelfPlayManager.ReportGoal(1); // Right team scores
        else
            SelfPlayManager.ReportGoal(0); // Left team scores
    }
}
