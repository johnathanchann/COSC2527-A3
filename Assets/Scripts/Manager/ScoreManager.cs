using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
// using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    int ScoreOfRightSide;
    int ScoreOfLeftSide;

    public GameManager GameManager;
    public SoundFxManager SoundFxManager;
    public GameObject GoalTextEffect;
    public TextMeshProUGUI ScoreBoardText;

    public void AddLeftScore()
    {
        ScoreOfLeftSide++;
        OnGoalScored();
    }

    IEnumerator Delay(System.Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }

    public void AddRightScore()
    {
        ScoreOfRightSide++;
        OnGoalScored();
    }

    private void OnGoalScored()
    {
        if (SoundFxManager != null)
        {
            SoundFxManager.PlayGoalScored();
        }
        if (GoalTextEffect != null)
        {
            GoalTextEffect.SetActive(true);
            StartCoroutine(Delay(() => { GoalTextEffect.SetActive(false); }, 2.0f));
        }
        if (ScoreBoardText != null)
        {
            ScoreBoardText.text = $"<mark padding=“10, 10, 0, 0”>{ScoreOfLeftSide} : {ScoreOfRightSide}</mark>";
        }
        GameManager.Refresh();
    }
}
