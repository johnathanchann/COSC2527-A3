using UnityEngine;

public class GoalZone : MonoBehaviour
{
    [Header("Which side is this?")]
    [SerializeField] bool isLeftGoal;

    public ScoreManager ScoreManager;
    public GameObject ball;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != ball) return;
        if (isLeftGoal)
            ScoreManager.AddRightScore();
        else
            ScoreManager.AddLeftScore();
    }
}
